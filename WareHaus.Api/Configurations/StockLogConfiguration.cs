using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WareHaus.Api.Models;

namespace WareHaus.Api.Configurations;

public class StockLogConfiguration : IEntityTypeConfiguration<StockLogs>
{
    public void Configure(EntityTypeBuilder<StockLogs> builder)
    {
        builder.HasKey(log => log.Id);

        builder.Property(log => log.MovementType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(log => log.Quantity)
            .IsRequired();

        builder.Property(log => log.StockAfterMovement)
            .IsRequired();

        builder.HasOne(log => log.Products)
            .WithMany()
            .HasForeignKey(log => log.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(log => log.Shelves)
            .WithMany()
            .HasForeignKey(log => log.ShelfId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}