using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WareHaus.Api.Models;

namespace WareHaus.Api.Configurations;

public class StockConfiguration : IEntityTypeConfiguration<Stocks>
{
    public void Configure(EntityTypeBuilder<Stocks> builder)
    {
        builder.HasKey(stock => new
        {
            stock.ShelfId,
            stock.ProductId
        });

        builder.Property(stock => stock.Quantity)
            .IsRequired();

        builder.HasOne(stock => stock.Shelves)
            .WithMany(shelf => shelf.Stocks)
            .HasForeignKey(stock => stock.ShelfId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(stock => stock.Products)
            .WithMany(product => product.Stocks)
            .HasForeignKey(stock => stock.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}