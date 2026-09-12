using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Persistence;

public class CatalogDbContext : DbContext
{
    public CatalogDbContext(
        DbContextOptions<CatalogDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.Description)
                .HasMaxLength(2000);

            entity.Property(x => x.Category)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.Brand)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.Price)
                .HasPrecision(18, 2);

            entity.HasIndex(x => x.Category);

            entity.HasIndex(x => x.Brand);
        });
    }
}
