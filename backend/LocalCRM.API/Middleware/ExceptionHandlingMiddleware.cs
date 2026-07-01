using System.Text.Json;

namespace LocalCRM.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _env;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IWebHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception has occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = exception switch
        {
            Exception e when e.Message == "Entity not found" => StatusCodes.Status404NotFound,
            Exception e when e.Message == "Concurrency conflict" => StatusCodes.Status409Conflict,
            Exception e when e.Message == "Invalid credentials" => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };

        // Suppress details in production
        var message = (_env.IsDevelopment() || code != 500) ? exception.Message : "An internal server error occurred. Please contact support.";

        var result = JsonSerializer.Serialize(new
        {
            code = code,
            message = message,
            traceId = context.TraceIdentifier
        });

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = code;

        await context.Response.WriteAsync(result);
    }
}
