using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WareHaus.Api.Models;

namespace WareHaus.Api.Configurations;

public class SalesOrderConfiguration : IEntityTypeConfiguration<SalesOrders>
{
    public void Configure(EntityTypeBuilder<SalesOrders> builder)
    {
        builder.ToTable("SalesOrders");

        builder.HasKey(salesOrder => salesOrder.Id);

        builder.HasIndex(salesOrder => salesOrder.SONumber)
            .IsUnique();

        builder.Property(salesOrder => salesOrder.SONumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(salesOrder => salesOrder.CustomerName)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(salesOrder => salesOrder.ShippingAddress)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(salesOrder => salesOrder.Courier)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(salesOrder => salesOrder.TrackingNumber)
            .HasMaxLength(100);

        builder.Property(salesOrder => salesOrder.Status)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(salesOrder => salesOrder.OrderDate)
            .IsRequired();

        builder.Property(salesOrder => salesOrder.RequiredDeliveryDate)
            .IsRequired();

        builder.HasMany(salesOrder => salesOrder.SOItems)
            .WithOne(item => item.SalesOrder)
            .HasForeignKey(item => item.SalesOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(salesOrder => salesOrder.PickingTasks)
            .WithOne(task => task.SalesOrder)
            .HasForeignKey(task => task.SalesOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(salesOrder => salesOrder.PackingTasks)
            .WithOne(task => task.SalesOrder)
            .HasForeignKey(task => task.SalesOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(salesOrder => salesOrder.Shipments)
            .WithOne(shipment => shipment.SalesOrder)
            .HasForeignKey(shipment => shipment.SalesOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(salesOrder => salesOrder.DeletedAt == null);
    }
}