using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ServiceTrack.Api.Middleware.Logging;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

    public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var requestId = Guid.NewGuid().ToString();
        var route = context.Request.Path;
        var method = context.Request.Method;
        var queryString = context.Request.QueryString.ToString();
        var requestBody = await ReadRequestBodyAsync(context.Request);
        
        var stopwatch = Stopwatch.StartNew();
        
        // Log request
        _logger.LogInformation(
            "Request {RequestId}: {Method} {Route}{QueryString} - Body: {RequestBody}",
            requestId, method, route, queryString, requestBody);

        // Capture the original response body stream
        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            
            // Read the response body
            responseBody.Seek(0, SeekOrigin.Begin);
            var responseBodyText = await new StreamReader(responseBody).ReadToEndAsync();
            
            // Copy the response body back to the original stream
            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);
            
            var statusCode = context.Response.StatusCode;
            var statusCodeColor = GetStatusCodeColor(statusCode);
            
            // Log response
            _logger.LogInformation(
                "Response {RequestId}: {Method} {Route} - Status: {StatusCodeColor}{StatusCode}\x1b[0m - Time: {ElapsedMilliseconds}ms - Body: {ResponseBody}",
                requestId, method, route, statusCodeColor, statusCode, stopwatch.ElapsedMilliseconds, responseBodyText);
        }
    }

    private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
    {
        request.EnableBuffering();
        
        using var reader = new StreamReader(
            request.Body,
            encoding: Encoding.UTF8,
            detectEncodingFromByteOrderMarks: false,
            leaveOpen: true);
            
        var body = await reader.ReadToEndAsync();
        request.Body.Position = 0;
        
        return body;
    }

    private static string GetStatusCodeColor(int statusCode)
    {
        return statusCode switch
        {
            >= 200 and < 300 => "\x1b[32m", // Green for success
            >= 300 and < 400 => "\x1b[33m", // Yellow for redirects
            >= 400 and < 500 => "\x1b[35m", // Magenta for client errors
            >= 500 => "\x1b[31m",           // Red for server errors
            _ => "\x1b[0m"                  // Reset for unknown
        };
    }
} 