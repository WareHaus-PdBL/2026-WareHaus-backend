using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WareHaus.Api.Models;

namespace WareHaus.Api.Configurations;

public class ZoneConfiguration : IEntityTypeConfiguration<Zones>
{
    public void Configure(EntityTypeBuilder<Zones> builder)
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
            .WithOne(shelf => shelf.Zones)
            .HasForeignKey(shelf => shelf.ZoneId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}