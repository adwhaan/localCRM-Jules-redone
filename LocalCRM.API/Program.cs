using LocalCRM.Application;
using LocalCRM.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add services to the container.
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

app.UseMiddleware<LocalCRM.API.Middleware.ExceptionHandlingMiddleware>();

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
