using System.Text.Json;
using Catalog.Application.Interfaces;
using Catalog.Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;

namespace Catalog.Infrastructure.Caching;

public class ProductCacheService : IProductCacheService
{
    private readonly IDistributedCache _cache;

    private static readonly TimeSpan CacheDuration =
        TimeSpan.FromMinutes(5);

    public ProductCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<Product?> GetAsync(Guid id)
    {
        var cacheKey = GetCacheKey(id);

        var cachedProduct = await _cache.GetStringAsync(cacheKey);

        if (cachedProduct is null)
        {
            return null;
        }

        return JsonSerializer.Deserialize<Product>(cachedProduct);
    }

    public async Task SetAsync(Product product)
    {
        var cacheKey = GetCacheKey(product.Id);

        var serializedProduct =
            JsonSerializer.Serialize(product);

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = CacheDuration
        };

        await _cache.SetStringAsync(
            cacheKey,
            serializedProduct,
            options);
    }

    public async Task RemoveAsync(Guid id)
    {
        var cacheKey = GetCacheKey(id);

        await _cache.RemoveAsync(cacheKey);
    }

    private static string GetCacheKey(Guid id)
    {
        return $"product:{id}";
    }
}
