using Catalog.Application.Interfaces;
using Catalog.Domain.Entities;

namespace Catalog.IntegrationTests.Fakes;

public class InMemoryProductRepository : IProductRepository
{
    private readonly Dictionary<Guid, Product> _products = new();

    public Task<Product?> GetByIdAsync(Guid id)
    {
        _products.TryGetValue(id, out var product);

        return Task.FromResult(product);
    }

    public Task<IReadOnlyList<Product>> GetAllAsync()
    {
        IReadOnlyList<Product> products =
            _products.Values.ToList();

        return Task.FromResult(products);
    }

    public Task<IReadOnlyList<Product>> SearchAsync(
        string query)
    {
        var products = _products.Values
            .Where(product =>
                product.Name.Contains(
                    query,
                    StringComparison.OrdinalIgnoreCase) ||
                (product.Description?.Contains(
                    query,
                    StringComparison.OrdinalIgnoreCase)
                    ?? false))
            .ToList();

        return Task.FromResult<
            IReadOnlyList<Product>>(products);
    }

    public Task AddAsync(Product product)
    {
        _products[product.Id] = product;

        return Task.CompletedTask;
    }

    public Task UpdateAsync(Product product)
    {
        _products[product.Id] = product;

        return Task.CompletedTask;
    }

    public Task DeleteAsync(Product product)
    {
        _products.Remove(product.Id);

        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(Guid id)
    {
        return Task.FromResult(
            _products.ContainsKey(id));
    }
}
