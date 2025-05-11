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
            RoleValidationException => (HttpStatusCode.BadRequest, exception.Message),
            RoleNameAlreadyExistsException => (HttpStatusCode.Conflict, exception.Message),
            RoleNotFoundException => (HttpStatusCode.NotFound, exception.Message),
            RoleInUseException => (HttpStatusCode.Conflict, exception.Message),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred")
        };

        _logger.LogError(exception, "An error occurred: {Message}", exception.Message);
        
        response.StatusCode = (int)statusCode;
        var result = JsonSerializer.Serialize(new { error = message });
        await response.WriteAsync(result);
    }
} 