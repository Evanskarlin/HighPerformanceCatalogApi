using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Catalog.Api.Health;

public class ElasticsearchHealthCheck : IHealthCheck
{
    private readonly ElasticsearchClient _client;

    public ElasticsearchHealthCheck(
        ElasticsearchClient client)
    {
        _client = client;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response =
                await _client.PingAsync(cancellationToken);

            if (response.IsValidResponse)
            {
                return HealthCheckResult.Healthy(
                    "Elasticsearch is available.");
            }

            return HealthCheckResult.Unhealthy(
                "Elasticsearch is unavailable.");
        }
        catch (Exception exception)
        {
            return HealthCheckResult.Unhealthy(
                "Elasticsearch is unavailable.",
                exception);
        }
    }
}
