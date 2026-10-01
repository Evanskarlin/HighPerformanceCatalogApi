using Catalog.Domain.Entities;

namespace Catalog.UnitTests.Domain;

public class ProductTests
{
    [Fact]
    public void Product_CanBeCreatedWithValidProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var now = DateTime.UtcNow;

        // Act
        var product = new Product
        {
            Id = id,
            Name = "Wireless Mouse",
            Description = "Ergonomic wireless mouse",
            Category = "Electronics",
            Brand = "Test Brand",
            Price = 49.99m,
            Stock = 25,
            CreatedAt = now,
            UpdatedAt = now
        };

        // Assert
        Assert.Equal(id, product.Id);
        Assert.Equal("Wireless Mouse", product.Name);
        Assert.Equal(
            "Ergonomic wireless mouse",
            product.Description);
        Assert.Equal("Electronics", product.Category);
        Assert.Equal("Test Brand", product.Brand);
        Assert.Equal(49.99m, product.Price);
        Assert.Equal(25, product.Stock);
        Assert.Equal(now, product.CreatedAt);
        Assert.Equal(now, product.UpdatedAt);
    }

    [Fact]
    public void Product_DescriptionCanBeNull()
    {
        // Arrange & Act
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Mechanical Keyboard",
            Description = null,
            Category = "Electronics",
            Brand = "Test Brand",
            Price = 99.99m,
            Stock = 10,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Assert
        Assert.Null(product.Description);
    }

    [Fact]
    public void Product_PriceSupportsDecimalValues()
    {
        // Arrange & Act
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "USB Hub",
            Category = "Electronics",
            Brand = "Test Brand",
            Price = 29.95m,
            Stock = 15,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Assert
        Assert.Equal(29.95m, product.Price);
    }
}
