using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WareHaus.Api.Models;

namespace WareHaus.Api.Configurations;

public class StockConfiguration : IEntityTypeConfiguration<Stock>
{
    public void Configure(EntityTypeBuilder<Stock> builder)
    {
        builder.HasKey(stock => new
        {
            stock.ShelfId,
            stock.ProductId
        });

        builder.Property(stock => stock.Quantity)
            .IsRequired();

        builder.HasOne(stock => stock.Shelf)
            .WithMany(shelf => shelf.Stocks)
            .HasForeignKey(stock => stock.ShelfId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(stock => stock.Product)
            .WithMany(product => product.Stocks)
            .HasForeignKey(stock => stock.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}