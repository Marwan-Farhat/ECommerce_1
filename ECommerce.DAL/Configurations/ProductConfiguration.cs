using ECommerce.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.DAL.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(p => p.Price)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(p => p.StockQuantity)
                .HasDefaultValue(0);

            builder.Property(p => p.ImageUrl)
                .HasMaxLength(500);

            builder.Property(p => p.IsActive)
                .HasDefaultValue(true);

            builder.Property(p => p.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(
                new Product { Id = 1, Name = "Laptop Pro", Description = "High performance laptop", Price = 1299.99m, StockQuantity = 50, CategoryId = 1, IsActive = true, CreatedAt = new DateTime(2024, 1, 1) },
                new Product { Id = 2, Name = "Wireless Mouse", Description = "Ergonomic wireless mouse", Price = 29.99m, StockQuantity = 200, CategoryId = 1, IsActive = true, CreatedAt = new DateTime(2024, 1, 1) },
                new Product { Id = 3, Name = "T-Shirt Classic", Description = "100% cotton classic t-shirt", Price = 19.99m, StockQuantity = 150, CategoryId = 2, IsActive = true, CreatedAt = new DateTime(2024, 1, 1) },
                new Product { Id = 4, Name = "C# Programming Guide", Description = "Complete guide to C# programming", Price = 49.99m, StockQuantity = 75, CategoryId = 3, IsActive = true, CreatedAt = new DateTime(2024, 1, 1) }
            );
        }
    }
}
