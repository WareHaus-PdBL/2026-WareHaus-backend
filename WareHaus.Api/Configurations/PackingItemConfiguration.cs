using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WareHaus.Api.Models;

namespace WareHaus.Api.Configurations;

public class PackingItemConfiguration : IEntityTypeConfiguration<PackingItems>
{
    public void Configure(EntityTypeBuilder<PackingItems> builder)
    {
        builder.ToTable("PackingItems");

        builder.HasKey(item => item.Id);

        builder.Property(item => item.PackingTaskId)
            .IsRequired();

        builder.Property(item => item.ProductId)
            .IsRequired();

        builder.Property(item => item.QtyExpected)
            .IsRequired();

        builder.Property(item => item.QtyVerified)
            .IsRequired();

        builder.Property(item => item.ExpectedBarcode)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(item => item.ScannedBarcode)
            .HasMaxLength(100);

        builder.Property(item => item.IsVerified)
            .IsRequired();

        builder.HasOne(item => item.PackingTask)
            .WithMany(task => task.PackingItems)
            .HasForeignKey(item => item.PackingTaskId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(item => item.Product)
            .WithMany()
            .HasForeignKey(item => item.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(item => item.DeletedAt == null);
    }
}