using LocalCRM.Application;
using LocalCRM.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Seed data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<LocalCRM.Infrastructure.Persistence.LocalCRMContext>();
    var identityService = scope.ServiceProvider.GetRequiredService<LocalCRM.Application.Common.Interfaces.IIdentityService>();
    context.Database.EnsureCreated();
    await LocalCRM.Infrastructure.Persistence.LocalCRMContextSeed.SeedDefaultDataAsync(context, identityService);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<LocalCRM.API.Middleware.ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
