using System.Diagnostics;

namespace Catalog.Api.Middleware;

public class RequestPerformanceMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestPerformanceMiddleware> _logger;

    public RequestPerformanceMiddleware(
        RequestDelegate next,
        ILogger<RequestPerformanceMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        context.Response.OnStarting(() =>
        {
            var elapsedMilliseconds =
                stopwatch.Elapsed.TotalMilliseconds;

            context.Response.Headers[
                "X-Response-Time-Ms"] =
                elapsedMilliseconds.ToString("F2");

            return Task.CompletedTask;
        });

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            _logger.LogInformation(
                "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMilliseconds:F2} ms",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.Elapsed.TotalMilliseconds);
        }
    }
}