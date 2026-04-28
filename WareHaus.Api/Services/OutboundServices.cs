using Microsoft.EntityFrameworkCore;
using WareHaus.API.Data;
using WareHaus.Api.DTOs;
using WareHaus.Api.Models;

namespace WareHaus.Api.Services;

public class OutboundServices
{
    private readonly AppDbContext _context;

    public OutboundServices(AppDbContext context)
    {
        _context = context;
    }

    public async Task<SalesOrders> CreateSalesOrderAsync(CreateSalesOrderDto dto)
    {
        var salesOrderExists = await _context.SalesOrders
            .AnyAsync(so => so.SONumber == dto.SONumber);

        if (salesOrderExists)
        {
            throw new InvalidOperationException("Nomor Sales Order sudah digunakan.");
        }

        var salesOrder = new SalesOrders
        {
            Id = Guid.NewGuid(),
            SONumber = dto.SONumber,
            CustomerName = dto.CustomerName,
            Status = "Created",
            CreatedAt = DateTime.UtcNow
        };

        foreach (var item in dto.Items)
        {
            var productExists = await _context.Products
                .AnyAsync(product => product.Id == item.ProductId);

            if (!productExists)
            {
                throw new KeyNotFoundException($"Product dengan ID {item.ProductId} tidak ditemukan.");
            }

            salesOrder.SOItems.Add(new SOItems
            {
                Id = Guid.NewGuid(),
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                PackedQuantity = 0,
                CreatedAt = DateTime.UtcNow
            });
        }

        _context.SalesOrders.Add(salesOrder);
        await _context.SaveChangesAsync();

        return salesOrder;
    }

    public async Task<PackingLogs> PackItemAsync(PackingDto dto)
    {
        var salesOrderItem = await _context.SOItems
            .FirstOrDefaultAsync(item =>
                item.SalesOrderId == dto.SalesOrderId &&
                item.ProductId == dto.ProductId);

        if (salesOrderItem == null)
        {
            throw new KeyNotFoundException("Item Sales Order tidak ditemukan.");
        }

        var remainingQuantity = salesOrderItem.Quantity - salesOrderItem.PackedQuantity;

        if (dto.QuantityPacked > remainingQuantity)
        {
            throw new InvalidOperationException("Jumlah packing melebihi jumlah item yang belum dipacking.");
        }

        salesOrderItem.PackedQuantity += dto.QuantityPacked;
        salesOrderItem.UpdatedAt = DateTime.UtcNow;

        var packingLog = new PackingLogs
        {
            Id = Guid.NewGuid(),
            SalesOrderId = dto.SalesOrderId,
            ProductId = dto.ProductId,
            QuantityPacked = dto.QuantityPacked,
            PackedBy = dto.PackedBy,
            PackedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        _context.PackingLogs.Add(packingLog);

        var salesOrder = await _context.SalesOrders
            .Include(so => so.SOItems)
            .FirstOrDefaultAsync(so => so.Id == dto.SalesOrderId);

        if (salesOrder != null && salesOrder.SOItems.All(item => item.PackedQuantity >= item.Quantity))
        {
            salesOrder.Status = "Packed";
            salesOrder.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return packingLog;
    }

    public async Task<ShippingLogs> ShipOrderAsync(ShippingDto dto)
    {
        var salesOrder = await _context.SalesOrders
            .Include(so => so.SOItems)
            .FirstOrDefaultAsync(so => so.Id == dto.SalesOrderId);

        if (salesOrder == null)
        {
            throw new KeyNotFoundException("Sales Order tidak ditemukan.");
        }

        var isFullyPacked = salesOrder.SOItems.All(item => item.PackedQuantity >= item.Quantity);

        if (!isFullyPacked)
        {
            throw new InvalidOperationException("Sales Order belum selesai dipacking.");
        }

        var shippingLog = new ShippingLogs
        {
            Id = Guid.NewGuid(),
            SalesOrderId = dto.SalesOrderId,
            CourierName = dto.CourierName,
            TrackingNumber = dto.TrackingNumber,
            ShippingStatus = "Shipped",
            ShippedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        salesOrder.Status = "Shipped";
        salesOrder.UpdatedAt = DateTime.UtcNow;

        _context.ShippingLogs.Add(shippingLog);

        await _context.SaveChangesAsync();

        return shippingLog;
    }
}