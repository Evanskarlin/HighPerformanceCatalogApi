using Catalog.Application.Interfaces;
using Catalog.Infrastructure.Repositories;
using Catalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Catalog.Infrastructure.Caching;
using Elastic.Clients.Elasticsearch;
using Catalog.Infrastructure.Search;
using Catalog.Api.Health;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CatalogDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("PostgreSql")));

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration =
        builder.Configuration.GetConnectionString("Redis");

    options.InstanceName = "Catalog:";
});

builder.Services.AddScoped<IProductSearchService, ElasticsearchProductSearchService>();

var elasticsearchUrl =
    builder.Configuration.GetConnectionString("Elasticsearch")
    ?? throw new InvalidOperationException(
        "Elasticsearch connection string is not configured.");

var elasticsearchSettings =
    new ElasticsearchClientSettings(
        new Uri(elasticsearchUrl));

builder.Services.AddSingleton(
    new ElasticsearchClient(elasticsearchSettings));

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductCacheService, ProductCacheService>();
builder.Services.AddScoped<IProductSearchCacheService, ProductSearchCacheService>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services
    .AddHealthChecks()
    .AddCheck<PostgreSqlHealthCheck>(
        "postgresql",
        tags: new[] { "database", "ready" })
    .AddCheck<RedisHealthCheck>(
        "redis",
        tags: new[] { "cache", "ready" })
    .AddCheck<ElasticsearchHealthCheck>(
        "elasticsearch",
        tags: new[] { "search", "ready" });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var productSearchService =
        scope.ServiceProvider
            .GetRequiredService<IProductSearchService>();

    await productSearchService.CreateIndexAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/health/elasticsearch",
    async (ElasticsearchClient client) =>
    {
        var response = await client.PingAsync();

        return response.IsValidResponse
            ? Results.Ok(new { status = "healthy" })
            : Results.Problem(
                "Elasticsearch is unavailable.");
    });

app.MapHealthChecks(
    "/health",
    new HealthCheckOptions
    {
        ResponseWriter =
            HealthCheckResponseWriter.WriteResponse
    });

app.Run();

public partial class Program
{
}
