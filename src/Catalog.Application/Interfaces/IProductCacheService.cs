using Catalog.Domain.Entities;

namespace Catalog.Application.Interfaces;

public interface IProductCacheService
{
    Task<Product?> GetAsync(Guid id);

    Task SetAsync(Product product);
}
