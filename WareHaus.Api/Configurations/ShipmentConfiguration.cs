using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WareHaus.Api.Models;

namespace WareHaus.Api.Configurations;

public class ShipmentConfiguration : IEntityTypeConfiguration<Shipments>
{
    public void Configure(EntityTypeBuilder<Shipments> builder)
    {
        builder.ToTable("Shipments");

        builder.HasKey(shipment => shipment.Id);

        builder.HasIndex(shipment => shipment.ShippingLabelNumber)
            .IsUnique();

        builder.Property(shipment => shipment.PackingTaskId)
            .IsRequired();

        builder.Property(shipment => shipment.SalesOrderId)
            .IsRequired();

        builder.Property(shipment => shipment.ShippingLabelNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(shipment => shipment.CourierName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(shipment => shipment.TrackingNumber)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(shipment => shipment.ShippingLabelUrl)
            .HasMaxLength(255);

        builder.Property(shipment => shipment.CustomerNameSnapshot)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(shipment => shipment.ShippingAddressSnapshot)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(shipment => shipment.Status)
            .HasMaxLength(30)
            .IsRequired();

        builder.HasOne(shipment => shipment.PackingTask)
            .WithMany(task => task.Shipments)
            .HasForeignKey(shipment => shipment.PackingTaskId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(shipment => shipment.SalesOrder)
            .WithMany(order => order.Shipments)
            .HasForeignKey(shipment => shipment.SalesOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(shipment => shipment.DeletedAt == null);
    }
}