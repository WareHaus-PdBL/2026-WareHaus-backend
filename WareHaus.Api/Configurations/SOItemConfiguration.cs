using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WareHaus.Api.Models;

namespace WareHaus.Api.Configurations;

public class SOItemConfiguration : IEntityTypeConfiguration<SOItems>
{
    public void Configure(EntityTypeBuilder<SOItems> builder)
    {
        builder.ToTable("SOItems");

        builder.HasKey(item => item.Id);

        builder.Property(item => item.SalesOrderId)
            .IsRequired();

        builder.Property(item => item.ProductId)
            .IsRequired();

        builder.Property(item => item.QtyOrdered)
            .IsRequired();

        builder.Property(item => item.QtyPicked)
            .IsRequired();

        builder.Property(item => item.QtyVerified)
            .IsRequired();

        builder.Property(item => item.UnitOfMeasureSnapshot)
            .HasMaxLength(30)
            .IsRequired();

        builder.HasOne(item => item.SalesOrder)
            .WithMany(order => order.SOItems)
            .HasForeignKey(item => item.SalesOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(item => item.Product)
            .WithMany()
            .HasForeignKey(item => item.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(item => item.DeletedAt == null);
    }
}