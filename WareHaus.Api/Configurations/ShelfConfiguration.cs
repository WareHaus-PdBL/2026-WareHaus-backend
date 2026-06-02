using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WareHaus.Api.Models;

namespace WareHaus.Api.Configurations;

public class ShelfConfiguration : IEntityTypeConfiguration<Shelves>
{
    public void Configure(EntityTypeBuilder<Shelves> builder)
    {
        builder.HasKey(shelf => shelf.Id);

        builder.HasIndex(shelf => shelf.ShelfCode)
            .IsUnique();

        builder.Property(shelf => shelf.ShelfCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(shelf => shelf.QRCodePath)
            .HasMaxLength(255);
    }
}