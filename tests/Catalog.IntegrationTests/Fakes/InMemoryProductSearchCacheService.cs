using Catalog.Application.Interfaces;
using Catalog.Application.Search;

namespace Catalog.IntegrationTests.Fakes;

public class InMemoryProductSearchCacheService
    : IProductSearchCacheService
{
    private readonly Dictionary<
        string,
        IReadOnlyList<ProductSearchDocument>> _cache =
        new(StringComparer.OrdinalIgnoreCase);

    public Task<IReadOnlyList<ProductSearchDocument>?> GetAsync(
        string query)
    {
        var key = Normalize(query);

        _cache.TryGetValue(
            key,
            out var products);

        return Task.FromResult(products);
    }

    public Task SetAsync(
        string query,
        IReadOnlyList<ProductSearchDocument> products)
    {
        _cache[Normalize(query)] = products;

        return Task.CompletedTask;
    }

    private static string Normalize(string query)
    {
        return query.Trim().ToLowerInvariant();
    }
}
