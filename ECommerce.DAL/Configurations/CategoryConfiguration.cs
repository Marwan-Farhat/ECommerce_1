using ECommerce.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.DAL.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Description)
                .HasMaxLength(500);

            builder.Property(c => c.IsActive)
                .HasDefaultValue(true);

            builder.Property(c => c.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasData(
                new Category { Id = 1, Name = "Electronics", Description = "Electronic devices and accessories", IsActive = true, CreatedAt = new DateTime(2024, 1, 1) },
                new Category { Id = 2, Name = "Clothing", Description = "Fashion and apparel", IsActive = true, CreatedAt = new DateTime(2024, 1, 1) },
                new Category { Id = 3, Name = "Books", Description = "Books and educational material", IsActive = true, CreatedAt = new DateTime(2024, 1, 1) }
            );
        }
    }
}
