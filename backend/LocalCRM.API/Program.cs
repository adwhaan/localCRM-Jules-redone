using LocalCRM.Application;
using LocalCRM.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// 1. Hardened CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("ProductionPolicy", policy =>
    {
        // In production, these would be loaded from configuration
        policy.WithOrigins("http://localhost:4200", "http://localhost:5001")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// 2. Rate Limiting (OWASP: Prevents Brute Force/DoS)
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1)
            }));

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", cancellationToken: token);
    };
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();

var app = builder.Build();

// Seed data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<LocalCRM.Infrastructure.Persistence.LocalCRMContext>();
    var identityService = scope.ServiceProvider.GetRequiredService<LocalCRM.Application.Common.Interfaces.IIdentityService>();
    context.Database.Migrate();
    await LocalCRM.Infrastructure.Persistence.LocalCRMContextSeed.SeedDefaultDataAsync(context, identityService);
}

// 3. Global Error Handling (Refined for Production)
app.UseMiddleware<LocalCRM.API.Middleware.ExceptionHandlingMiddleware>();

// 4. Security Headers (OWASP: Anti-Clickjacking, XSS Protection)
app.UseHsts(); // Enforce HTTPS
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("Content-Security-Policy", "default-src 'self';");
    context.Response.Headers.Append("Referrer-Policy", "no-referrer");
    await next();
});

if (app.Environment.IsDevelopment())
{
    app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
}
else
{
    app.UseCors("ProductionPolicy");
}

app.UseHttpsRedirection();
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
