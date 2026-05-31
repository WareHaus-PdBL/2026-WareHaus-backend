using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WareHaus.Api.Models;

namespace WareHaus.Api.Configurations;

public class PackingTaskConfiguration : IEntityTypeConfiguration<PackingTasks>
{
    public void Configure(EntityTypeBuilder<PackingTasks> builder)
    {
        builder.ToTable("PackingTasks");

        builder.HasKey(task => task.Id);

        builder.HasIndex(task => task.PackingNumber)
            .IsUnique();

        builder.Property(task => task.PackingNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(task => task.SalesOrderId)
            .IsRequired();

        builder.Property(task => task.TotalPackage)
            .IsRequired();

        builder.Property(task => task.PackingStatus)
            .HasMaxLength(30)
            .IsRequired();

        builder.HasOne(task => task.SalesOrder)
            .WithMany(order => order.PackingTasks)
            .HasForeignKey(task => task.SalesOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(task => task.PackingItems)
            .WithOne(item => item.PackingTask)
            .HasForeignKey(item => item.PackingTaskId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(task => task.Shipments)
            .WithOne(shipment => shipment.PackingTask)
            .HasForeignKey(shipment => shipment.PackingTaskId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(task => task.DeletedAt == null);
    }
}