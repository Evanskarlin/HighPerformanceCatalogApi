using Catalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Catalog.Api.Health;

public class PostgreSqlHealthCheck : IHealthCheck
{
    private readonly CatalogDbContext _dbContext;

    public PostgreSqlHealthCheck(
        CatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var canConnect =
                await _dbContext.Database.CanConnectAsync(
                    cancellationToken);

            if (canConnect)
            {
                return HealthCheckResult.Healthy(
                    "PostgreSQL is available.");
            }

            return HealthCheckResult.Unhealthy(
                "PostgreSQL is unavailable.");
        }
        catch (Exception exception)
        {
            return HealthCheckResult.Unhealthy(
                "PostgreSQL is unavailable.",
                exception);
        }
    }
}
