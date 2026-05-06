using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WareHaus.Api.Models;

namespace WareHaus.Api.Configurations;

public class PackingItemConfiguration : IEntityTypeConfiguration<PackingItems>
{
    public void Configure(EntityTypeBuilder<PackingItems> builder)
    {
        builder.HasKey(item => item.Id);

        builder.Property(item => item.QtyVerified)
            .IsRequired();

        builder.HasOne(item => item.PackingTasks)
            .WithMany(task => task.PackingItems)
            .HasForeignKey(item => item.PackingTaskId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(item => item.Products)
            .WithMany()
            .HasForeignKey(item => item.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}