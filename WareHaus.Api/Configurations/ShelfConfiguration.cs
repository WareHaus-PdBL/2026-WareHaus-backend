using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WareHaus.API.Models;

namespace WareHaus.API.Configurations;

public class ShelfConfiguration : IEntityTypeConfiguration<Shelf>
{
    public void Configure(EntityTypeBuilder<Shelf> builder)
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