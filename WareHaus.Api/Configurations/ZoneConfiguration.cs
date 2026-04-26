using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WareHaus.API.Models;

namespace WareHaus.API.Configurations;

public class ZoneConfiguration : IEntityTypeConfiguration<Zone>
{
    public void Configure(EntityTypeBuilder<Zone> builder)
    {
        builder.HasKey(zone => zone.Id);

        builder.HasIndex(zone => zone.ZoneCode)
            .IsUnique();

        builder.Property(zone => zone.ZoneCode)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(zone => zone.ZoneName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(zone => zone.Category)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasMany(zone => zone.Shelves)
            .WithOne(shelf => shelf.Zone)
            .HasForeignKey(shelf => shelf.ZoneId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}