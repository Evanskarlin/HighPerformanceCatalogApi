using Catalog.Application.Search;

namespace Catalog.Application.Interfaces;

public interface IProductSearchCacheService
{
    Task<IReadOnlyList<ProductSearchDocument>?> GetAsync(
        string query);

    Task SetAsync(
        string query,
        IReadOnlyList<ProductSearchDocument> products);
}
