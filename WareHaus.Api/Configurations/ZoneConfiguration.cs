using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WareHaus.API.Models;

namespace WareHaus.API.Configurations;

public class ZoneConfiguration : IEntityTypeConfiguration<Zone>
{
    public void Configure(EntityTypeBuilder<Zone> entity)
    {
        entity.ToTable("Zones");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.ZoneCode)
            .HasMaxLength(50)
            .IsRequired();

        entity.Property(e => e.ZoneName)
            .HasMaxLength(150)
            .IsRequired();

        entity.Property(e => e.Description)
            .HasMaxLength(255);

        entity.Property(e => e.Category)
            .HasMaxLength(50)
            .IsRequired();

        entity.Property(e => e.TotalAisle)
            .IsRequired();

        entity.Property(e => e.ShelfPerAisle)
            .IsRequired();

        entity.Property(e => e.LevelPerShelf)
            .IsRequired();

        entity.Property(e => e.CreatedAt)
            .IsRequired();

        entity.HasIndex(e => e.ZoneCode)
            .IsUnique();

        entity.HasMany(e => e.Aisles)
            .WithOne(e => e.Zone)
            .HasForeignKey(e => e.ZoneId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasQueryFilter(e => e.DeletedAt == null);
    }
}