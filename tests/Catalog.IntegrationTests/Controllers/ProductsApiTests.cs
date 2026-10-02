using System.Net;
using System.Net.Http.Json;
using Catalog.Api.Models.Products;
using Catalog.Domain.Entities;
using Catalog.IntegrationTests.Infrastructure;

namespace Catalog.IntegrationTests.Controllers;

public class ProductsApiTests
    : IClassFixture<CatalogApiFactory>
{
    private readonly HttpClient _client;

    public ProductsApiTests(
        CatalogApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateProduct_ReturnsCreatedProduct()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "Integration Test Mouse",
            Description = "Created by integration test",
            Category = "Electronics",
            Brand = "Test Brand",
            Price = 49.99m,
            Stock = 20
        };

        // Act
        var response =
            await _client.PostAsJsonAsync(
                "/api/products",
                request);

        // Assert
        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);

        var product =
            await response.Content
                .ReadFromJsonAsync<Product>();

        Assert.NotNull(product);

        Assert.Equal(
            "Integration Test Mouse",
            product.Name);

        Assert.Equal(49.99m, product.Price);
        Assert.Equal(20, product.Stock);

        Assert.NotEqual(
            Guid.Empty,
            product.Id);
    }

    [Fact]
    public async Task GetProduct_AfterCreation_ReturnsProduct()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "Integration Test Keyboard",
            Description = "Mechanical keyboard",
            Category = "Electronics",
            Brand = "Test Brand",
            Price = 99.99m,
            Stock = 10
        };

        var createResponse =
            await _client.PostAsJsonAsync(
                "/api/products",
                request);

        var createdProduct =
            await createResponse.Content
                .ReadFromJsonAsync<Product>();

        Assert.NotNull(createdProduct);

        // Act
        var response =
            await _client.GetAsync(
                $"/api/products/{createdProduct.Id}");

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var product =
            await response.Content
                .ReadFromJsonAsync<Product>();

        Assert.NotNull(product);

        Assert.Equal(
            createdProduct.Id,
            product.Id);

        Assert.Equal(
            "Integration Test Keyboard",
            product.Name);
    }

    [Fact]
    public async Task GetProduct_WithUnknownId_ReturnsNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var response =
            await _client.GetAsync(
                $"/api/products/{id}");

        // Assert
        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_WithNegativePrice_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "Invalid Product",
            Description = "Invalid price",
            Category = "Electronics",
            Brand = "Test Brand",
            Price = -10m,
            Stock = 10
        };

        // Act
        var response =
            await _client.PostAsJsonAsync(
                "/api/products",
                request);

        // Assert
        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task DeleteProduct_AfterCreation_ReturnsNoContent()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "Product To Delete",
            Description = "Temporary product",
            Category = "Electronics",
            Brand = "Test Brand",
            Price = 25m,
            Stock = 5
        };

        var createResponse =
            await _client.PostAsJsonAsync(
                "/api/products",
                request);

        var product =
            await createResponse.Content
                .ReadFromJsonAsync<Product>();

        Assert.NotNull(product);

        // Act
        var deleteResponse =
            await _client.DeleteAsync(
                $"/api/products/{product.Id}");

        // Assert
        Assert.Equal(
            HttpStatusCode.NoContent,
            deleteResponse.StatusCode);

        var getResponse =
            await _client.GetAsync(
                $"/api/products/{product.Id}");

        Assert.Equal(
            HttpStatusCode.NotFound,
            getResponse.StatusCode);
    }
}
