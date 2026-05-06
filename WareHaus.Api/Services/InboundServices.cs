using Microsoft.EntityFrameworkCore;
using WareHaus.Api.Data;
using WareHaus.Api.DTOs;
using WareHaus.Api.Models;

namespace WareHaus.Api.Services;

public class InboundServices
{
    private readonly AppDbContext _context;

    public InboundServices(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PurchaseOrders> CreatePOAsync(CreatePurchaseOrderDto dto)
    {
        var purchaseOrder = new PurchaseOrders
        {
            PONumber = dto.PONumber,
            SupplierName = dto.SupplierName,
            Status = "Pending",
            OrderDate = DateTime.UtcNow
        };

        foreach (var item in dto.Items)
        {
            purchaseOrder.POItems.Add(new POItems
            {
                ProductId = item.ProductId,
                QtyExpected = item.QtyExpected,
                QtyReceived = 0
            });
        }

        _context.PurchaseOrders.Add(purchaseOrder);
        await _context.SaveChangesAsync();

        return purchaseOrder;
    }

    public async Task<ReceivingLogs> ReceiveItemAsync(ReceiveItemDto dto)
    {
        var poItem = await _context.POItems
            .FirstOrDefaultAsync(item => item.Id == dto.POItemId && item.DeletedAt == null);

        if (poItem == null)
        {
            throw new KeyNotFoundException("PO Item tidak ditemukan");
        }

        var receivingLog = new ReceivingLogs
        {
            PurchaseOrderId = poItem.PurchaseOrderId,
            POItemId = poItem.Id,
            ProductId = poItem.ProductId,
            QtyReceived = dto.QtyReceived,
            Condition = dto.Condition,
            ExpiryDate = dto.ExpiryDate,
            ReceivedAt = DateTime.UtcNow
        };

        poItem.QtyReceived += dto.QtyReceived;

        _context.ReceivingLogs.Add(receivingLog);
        await _context.SaveChangesAsync();

        return receivingLog;
    }

    public async Task<Stocks> PutawayAsync(PutawayDto dto)
    {
        var shelf = await _context.Shelves
            .FirstOrDefaultAsync(shelf => shelf.ShelfCode == dto.ShelfCode && shelf.DeletedAt == null);

        if (shelf == null)
        {
            throw new KeyNotFoundException("Rak tidak ditemukan");
        }

        var stock = await _context.Stocks
            .FirstOrDefaultAsync(stock =>
                stock.ShelfId == shelf.Id &&
                stock.ProductId == dto.ProductId &&
                stock.DeletedAt == null);

        if (stock == null)
        {
            stock = new Stocks
            {
                ShelfId = shelf.Id,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity
            };

            _context.Stocks.Add(stock);
        }
        else
        {
            stock.Quantity += dto.Quantity;
        }

        shelf.CurrentVolume += dto.Quantity;

        await _context.SaveChangesAsync();

        return stock;
    }
}