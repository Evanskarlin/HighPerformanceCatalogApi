using Catalog.Domain.Entities;
using Catalog.Application.Search;

namespace Catalog.Application.Interfaces;

public interface IProductSearchService
{
    Task CreateIndexAsync();

    Task IndexAsync(Product product);

    Task<IReadOnlyList<ProductSearchDocument>> SearchAsync(
    string query);
}
