using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WareHaus.Api.Models;

namespace WareHaus.Api.Configurations;

public class ShipmentConfiguration : IEntityTypeConfiguration<Shipments>
{
    public void Configure(EntityTypeBuilder<Shipments> builder)
    {
        builder.HasKey(shipment => shipment.Id);

        builder.Property(shipment => shipment.CourierName)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(shipment => shipment.TrackingNumber)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(shipment => shipment.ShippingLabelUrl)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(shipment => shipment.Status)
            .HasMaxLength(30)
            .IsRequired();

        builder.HasOne(shipment => shipment.PackingTasks)
            .WithMany(task => task.Shipments)
            .HasForeignKey(shipment => shipment.PackingTaskId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}