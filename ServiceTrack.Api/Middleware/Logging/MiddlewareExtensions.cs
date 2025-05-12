using Microsoft.AspNetCore.Builder;

namespace ServiceTrack.Api.Middleware.Logging;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestResponseLoggingMiddleware>();
    }
} 