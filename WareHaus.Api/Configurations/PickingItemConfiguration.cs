using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WareHaus.Api.Models;

namespace WareHaus.Api.Configurations;

public class PickingItemConfiguration : IEntityTypeConfiguration<PickingItems>
{
    public void Configure(EntityTypeBuilder<PickingItems> builder)
    {
        builder.ToTable("PickingItems");

        builder.HasKey(item => item.Id);

        builder.Property(item => item.PickingTaskId)
            .IsRequired();

        builder.Property(item => item.ProductId)
            .IsRequired();

        builder.Property(item => item.ShelfId)
            .IsRequired();

        builder.Property(item => item.QtyToPick)
            .IsRequired();

        builder.Property(item => item.QtyPicked)
            .IsRequired();

        builder.Property(item => item.UnitOfMeasureSnapshot)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(item => item.LocationSuggestion)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(item => item.ScannedShelfQrCode)
            .HasMaxLength(255);

        builder.HasOne(item => item.PickingTask)
            .WithMany(task => task.PickingItems)
            .HasForeignKey(item => item.PickingTaskId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(item => item.Product)
            .WithMany()
            .HasForeignKey(item => item.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(item => item.Shelf)
            .WithMany()
            .HasForeignKey(item => item.ShelfId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(item => item.DeletedAt == null);
    }
}