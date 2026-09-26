using System.Text.Json;
using Catalog.Application.Interfaces;
using Catalog.Application.Search;
using Microsoft.Extensions.Caching.Distributed;

namespace Catalog.Infrastructure.Caching;

public class ProductSearchCacheService
    : IProductSearchCacheService
{
    private readonly IDistributedCache _cache;

    private static readonly TimeSpan CacheDuration =
        TimeSpan.FromMinutes(2);

    public ProductSearchCacheService(
        IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<IReadOnlyList<ProductSearchDocument>?> GetAsync(
        string query)
    {
        var cacheKey = GetCacheKey(query);

        var cachedResults =
            await _cache.GetStringAsync(cacheKey);

        if (cachedResults is null)
        {
            return null;
        }

        return JsonSerializer.Deserialize<
            List<ProductSearchDocument>>(cachedResults);
    }

    public async Task SetAsync(
        string query,
        IReadOnlyList<ProductSearchDocument> products)
    {
        var cacheKey = GetCacheKey(query);

        var serializedProducts =
            JsonSerializer.Serialize(products);

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = CacheDuration
        };

        await _cache.SetStringAsync(
            cacheKey,
            serializedProducts,
            options);
    }

    private static string GetCacheKey(string query)
    {
        var normalizedQuery =
            query.Trim().ToLowerInvariant();

        return $"search:{normalizedQuery}";
    }
}
