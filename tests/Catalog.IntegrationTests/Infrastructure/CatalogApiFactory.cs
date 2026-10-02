using Catalog.Application.Interfaces;
using Catalog.IntegrationTests.Fakes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Catalog.IntegrationTests.Infrastructure;

public class CatalogApiFactory
    : WebApplicationFactory<Program>
{
    private static readonly KeyValuePair<string, string?>[]
        TestConfiguration =
        [
            new(
                "ConnectionStrings:PostgreSql",
                "Host=localhost;Port=5432;Database=test;Username=test;Password=test"
            ),
            new(
                "ConnectionStrings:Redis",
                "localhost:6379"
            ),
            new(
                "ConnectionStrings:Elasticsearch",
                "http://localhost:9200"
            )
        ];

    protected override IHost CreateHost(
        IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(
            configuration =>
            {
                configuration.AddInMemoryCollection(
                    TestConfiguration);
            });

        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IProductRepository>();
            services.RemoveAll<IProductCacheService>();
            services.RemoveAll<IProductSearchCacheService>();
            services.RemoveAll<IProductSearchService>();

            services.AddSingleton<
                IProductRepository,
                InMemoryProductRepository>();

            services.AddSingleton<
                IProductCacheService,
                InMemoryProductCacheService>();

            services.AddSingleton<
                IProductSearchCacheService,
                InMemoryProductSearchCacheService>();

            services.AddSingleton<
                IProductSearchService,
                InMemoryProductSearchService>();
        });
    }
}