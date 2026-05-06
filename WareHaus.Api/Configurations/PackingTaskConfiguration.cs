using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WareHaus.Api.Models;

namespace WareHaus.Api.Configurations;

public class PackingTaskConfiguration : IEntityTypeConfiguration<PackingTasks>
{
    public void Configure(EntityTypeBuilder<PackingTasks> builder)
    {
        builder.HasKey(task => task.Id);

        builder.Property(task => task.TotalPackage)
            .IsRequired();

        builder.Property(task => task.PackingStatus)
            .HasMaxLength(30)
            .IsRequired();

        builder.HasOne(task => task.SalesOrders)
            .WithMany(order => order.PackingTasks)
            .HasForeignKey(task => task.SOId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(task => task.PackingItems)
            .WithOne(item => item.PackingTasks)
            .HasForeignKey(item => item.PackingTaskId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(task => task.Shipments)
            .WithOne(shipment => shipment.PackingTasks)
            .HasForeignKey(shipment => shipment.PackingTaskId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}