using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WareHaus.API.Models;

namespace WareHaus.API.Configurations;

public class ShelfConfiguration : IEntityTypeConfiguration<Shelf>
{
    public void Configure(EntityTypeBuilder<Shelf> entity)
    {
        entity.ToTable("Shelves");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.AisleId)
            .IsRequired();

        entity.Property(e => e.ShelfCode)
            .HasMaxLength(50)
            .IsRequired();

        entity.Property(e => e.Capacity)
            .IsRequired();

        entity.Property(e => e.CurrentVolume)
            .IsRequired();

        entity.Property(e => e.QRCodePath)
            .HasMaxLength(255);

        entity.Property(e => e.CreatedAt)
            .IsRequired();

        entity.HasIndex(e => e.ShelfCode)
            .IsUnique();

        entity.HasOne(e => e.Aisle)
            .WithMany(e => e.Shelves)
            .HasForeignKey(e => e.AisleId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasMany(e => e.Stocks)
            .WithOne(e => e.Shelf)
            .HasForeignKey(e => e.ShelfId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasQueryFilter(e => e.DeletedAt == null);
    }
}