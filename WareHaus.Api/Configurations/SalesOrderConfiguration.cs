using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WareHaus.Api.Models;

namespace WareHaus.Api.Configurations;

public class SalesOrderConfiguration : IEntityTypeConfiguration<SalesOrders>
{
    public void Configure(EntityTypeBuilder<SalesOrders> builder)
    {
        builder.HasKey(salesOrder => salesOrder.Id);

        builder.HasIndex(salesOrder => salesOrder.SONumber)
            .IsUnique();

        builder.Property(salesOrder => salesOrder.SONumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(salesOrder => salesOrder.CustomerName)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(salesOrder => salesOrder.Status)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(salesOrder => salesOrder.OrderDate)
            .IsRequired();

        builder.HasMany(salesOrder => salesOrder.SOItems)
            .WithOne(item => item.SalesOrders)
            .HasForeignKey(item => item.SOId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(salesOrder => salesOrder.PackingTasks)
            .WithOne(task => task.SalesOrders)
            .HasForeignKey(task => task.SOId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}