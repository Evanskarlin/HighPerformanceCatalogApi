using Catalog.Domain.Entities;

namespace Catalog.Application.Interfaces;

public interface IProductSearchService
{
    Task CreateIndexAsync();

    Task IndexAsync(Product product);
}
