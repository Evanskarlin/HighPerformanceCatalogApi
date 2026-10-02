using Catalog.Application.Interfaces;
using Catalog.Domain.Entities;

namespace Catalog.IntegrationTests.Fakes;

public class InMemoryProductCacheService
    : IProductCacheService
{
    private readonly Dictionary<Guid, Product> _cache = new();

    public Task<Product?> GetAsync(Guid id)
    {
        _cache.TryGetValue(id, out var product);

        return Task.FromResult(product);
    }

    public Task SetAsync(Product product)
    {
        _cache[product.Id] = product;

        return Task.CompletedTask;
    }

    public Task RemoveAsync(Guid id)
    {
        _cache.Remove(id);

        return Task.CompletedTask;
    }
}
