using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WareHaus.Api.Models;

namespace WareHaus.Api.Configurations;

public class SOItemConfiguration : IEntityTypeConfiguration<SOItems>
{
    public void Configure(EntityTypeBuilder<SOItems> builder)
    {
        builder.HasKey(item => item.Id);

        builder.Property(item => item.QtyOrdered)
            .IsRequired();

        builder.Property(item => item.QtyPicked)
            .IsRequired();

        builder.HasOne(item => item.SalesOrders)
            .WithMany(order => order.SOItems)
            .HasForeignKey(item => item.SOId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(item => item.Products)
            .WithMany()
            .HasForeignKey(item => item.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}