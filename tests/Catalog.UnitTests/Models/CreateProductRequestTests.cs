using System.ComponentModel.DataAnnotations;
using Catalog.Api.Models.Products;

namespace Catalog.UnitTests.Models;

public class CreateProductRequestTests
{
    [Fact]
    public void ValidRequest_PassesValidation()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "Wireless Mouse",
            Description = "Ergonomic wireless mouse",
            Category = "Electronics",
            Brand = "Test Brand",
            Price = 49.99m,
            Stock = 25
        };

        // Act
        var results = Validate(request);

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void NegativePrice_FailsValidation()
    {
        // Arrange
        var request = CreateValidRequest();

        request.Price = -1m;

        // Act
        var results = Validate(request);

        // Assert
        Assert.Contains(
            results,
            result =>
                result.MemberNames.Contains(
                    nameof(CreateProductRequest.Price)));
    }

    [Fact]
    public void NegativeStock_FailsValidation()
    {
        // Arrange
        var request = CreateValidRequest();

        request.Stock = -1;

        // Act
        var results = Validate(request);

        // Assert
        Assert.Contains(
            results,
            result =>
                result.MemberNames.Contains(
                    nameof(CreateProductRequest.Stock)));
    }

    [Fact]
    public void NameLongerThan200Characters_FailsValidation()
    {
        // Arrange
        var request = CreateValidRequest();

        request.Name = new string('A', 201);

        // Act
        var results = Validate(request);

        // Assert
        Assert.Contains(
            results,
            result =>
                result.MemberNames.Contains(
                    nameof(CreateProductRequest.Name)));
    }

    private static CreateProductRequest CreateValidRequest()
    {
        return new CreateProductRequest
        {
            Name = "Wireless Mouse",
            Description = "Ergonomic wireless mouse",
            Category = "Electronics",
            Brand = "Test Brand",
            Price = 49.99m,
            Stock = 25
        };
    }

    private static List<ValidationResult> Validate(
        CreateProductRequest request)
    {
        var results =
            new List<ValidationResult>();

        var context =
            new ValidationContext(request);

        Validator.TryValidateObject(
            request,
            context,
            results,
            validateAllProperties: true);

        return results;
    }
}
