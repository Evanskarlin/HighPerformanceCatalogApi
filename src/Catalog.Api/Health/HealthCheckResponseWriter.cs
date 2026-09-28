using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Catalog.Api.Health;

public static class HealthCheckResponseWriter
{
    public static async Task WriteResponse(
        HttpContext context,
        HealthReport report)
    {
        context.Response.ContentType =
            "application/json";

        var response = new
        {
            status = report.Status.ToString(),

            durationMs =
                report.TotalDuration.TotalMilliseconds,

            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,

                status =
                    entry.Value.Status.ToString(),

                description =
                    entry.Value.Description,

                durationMs =
                    entry.Value.Duration.TotalMilliseconds
            })
        };

        var json =
            JsonSerializer.Serialize(
                response,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

        await context.Response.WriteAsync(json);
    }
}
