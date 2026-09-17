using System.ComponentModel.DataAnnotations;

namespace Catalog.Api.Models.Products;

public class UpdateProductRequest
{
    [Required]
    [MaxLength(200)]
    public required string Name { get; set; }

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(100)]
    public required string Category { get; set; }

    [Required]
    [MaxLength(100)]
    public required string Brand { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int Stock { get; set; }
}
