using Catalog.Application.Interfaces;
using Catalog.Application.Search;
using Catalog.Domain.Entities;

namespace Catalog.IntegrationTests.Fakes;

public class InMemoryProductSearchService
    : IProductSearchService
{
    private readonly Dictionary<
        Guid,
        ProductSearchDocument> _documents = new();

    public Task CreateIndexAsync()
    {
        return Task.CompletedTask;
    }

    public Task IndexAsync(Product product)
    {
        _documents[product.Id] =
            new ProductSearchDocument
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Category = product.Category,
                Brand = product.Brand,
                Price = product.Price,
                Stock = product.Stock
            };

        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id)
    {
        _documents.Remove(id);

        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<ProductSearchDocument>> SearchAsync(
        string query)
    {
        IReadOnlyList<ProductSearchDocument> results =
            _documents.Values
                .Where(product =>
                    product.Name.Contains(
                        query,
                        StringComparison.OrdinalIgnoreCase) ||
                    (product.Description?.Contains(
                        query,
                        StringComparison.OrdinalIgnoreCase)
                        ?? false))
                .ToList();

        return Task.FromResult(results);
    }
}
