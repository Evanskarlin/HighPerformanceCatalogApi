namespace Catalog.Domain.Entities;

public class Product
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }

    public required string Category { get; set; }

    public required string Brand { get; set; }

    public decimal Price { get; set; }

    public int Stock { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
