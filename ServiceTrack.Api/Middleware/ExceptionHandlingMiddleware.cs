using AuthApp.application.Exceptions;
using System.Net;
using System.Text.Json;

namespace ServiceTrack.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";
        
        var (statusCode, message) = exception switch
        {
            // Role-related exceptions (most specific)
            RoleValidationException => (HttpStatusCode.BadRequest, exception.Message),
            RoleNameAlreadyExistsException => (HttpStatusCode.Conflict, exception.Message),
            RoleNotFoundException => (HttpStatusCode.NotFound, exception.Message),
            RoleInUseException => (HttpStatusCode.Conflict, exception.Message),
            
            //Customer-related exceptions
            CustomerValidationException => (HttpStatusCode.BadRequest, exception.Message),
            CustomerNotFoundException => (HttpStatusCode.NotFound, exception.Message),
            CustomerInUseException => (HttpStatusCode.Conflict, exception.Message),
            CustomerAlreadyExistsException => (HttpStatusCode.Conflict, exception.Message),
            
            // Validation exceptions (more specific to general)
            ArgumentNullException => (HttpStatusCode.BadRequest, exception.Message),
            InvalidOperationException => (HttpStatusCode.BadRequest, exception.Message),
            ArgumentException => (HttpStatusCode.BadRequest, exception.Message),
            
            // Not found exceptions
            KeyNotFoundException => (HttpStatusCode.NotFound, exception.Message),
            
            // Default case (most general)
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred. Please try again later.")
        };

        var logLevel = statusCode == HttpStatusCode.InternalServerError ? LogLevel.Error : LogLevel.Warning;
        _logger.Log(logLevel, exception, "An error occurred: {Message}", exception.Message);
        
        response.StatusCode = (int)statusCode;
        var result = JsonSerializer.Serialize(new 
        { 
            error = message,
            statusCode = (int)statusCode,
            timestamp = DateTime.UtcNow
        });
        await response.WriteAsync(result);
    }
} 