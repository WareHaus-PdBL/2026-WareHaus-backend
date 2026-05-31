using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WareHaus.Api.Models;

namespace WareHaus.Api.Configurations;

public class PickingTaskConfiguration : IEntityTypeConfiguration<PickingTasks>
{
    public void Configure(EntityTypeBuilder<PickingTasks> builder)
    {
        builder.ToTable("PickingTasks");

        builder.HasKey(task => task.Id);

        builder.HasIndex(task => task.PickingNumber)
            .IsUnique();

        builder.Property(task => task.PickingNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(task => task.SalesOrderId)
            .IsRequired();

        builder.Property(task => task.TotalItems)
            .IsRequired();

        builder.Property(task => task.PickingStatus)
            .HasMaxLength(30)
            .IsRequired();

        builder.HasOne(task => task.SalesOrder)
            .WithMany(order => order.PickingTasks)
            .HasForeignKey(task => task.SalesOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(task => task.PickingItems)
            .WithOne(item => item.PickingTask)
            .HasForeignKey(item => item.PickingTaskId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(task => task.DeletedAt == null);
    }
}