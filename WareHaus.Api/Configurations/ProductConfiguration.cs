using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WareHaus.Api.Models;

namespace WareHaus.Api.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Products>
{
    public void Configure(EntityTypeBuilder<Products> builder)
    {
        builder.HasKey(product => product.Id);

        builder.HasIndex(product => product.SKU)
            .IsUnique();

        builder.HasIndex(product => product.Barcode)
            .IsUnique();

        builder.Property(product => product.SKU)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(product => product.ProductName)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(product => product.Barcode)
            .HasMaxLength(100);

        builder.Property(product => product.UnitOfMeasure)
            .HasMaxLength(30)
            .IsRequired();
    }
}