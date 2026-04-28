using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WareHaus.API.Data;
using WareHaus.Api.Models;
using WareHaus.Api.DTOs;
using System.Collections.Generic;

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
        var po = new PurchaseOrders
        {
            Id = Guid.NewGuid(),
            PONumber = dto.PONumber,
            SupplierName = dto.SupplierName,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        foreach (var item in dto.Items)
        {
            po.POItems.Add(new POItems
            {
                Id = Guid.NewGuid(),
                ProductId = item.ProductId,
                QtyExpected = item.QtyExpected,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }

        _context.PurchaseOrders.Add(po);
        await _context.SaveChangesAsync();
        return po;
    }

    public async Task<ReceivingLogs> ReceiveItemAsync(ReceiveItemDto dto)
    {
        var log = new ReceivingLogs
        {
            Id = Guid.NewGuid(),
            POItemId = dto.POItemId,
            QtyReceived = dto.QtyReceived,
            Condition = dto.Condition,
            ExpiryDate = dto.ExpiryDate,
            ReceivedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.ReceivingLogs.Add(log);
        await _context.SaveChangesAsync();
        return log;
    }

    public async Task<Stocks> PutawayAsync(PutawayDto dto)
    {
        var shelf = await _context.Shelves.FirstOrDefaultAsync(s => s.ShelfCode == dto.ShelfCode);
        if (shelf == null) throw new KeyNotFoundException("Rak tidak ditemukan");

        var stock = await _context.Stocks.FirstOrDefaultAsync(s => s.ShelfId == shelf.Id && s.ProductId == dto.ProductId);

        if (stock == null)
        {
            stock = new Stocks
            {
                Id = Guid.NewGuid(),
                ShelfId = shelf.Id,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.Stocks.Add(stock);
        }
        else
        {
            stock.Quantity += dto.Quantity;
            stock.UpdatedAt = DateTime.UtcNow;
            _context.Stocks.Update(stock);
        }

        shelf.CurrentVolume += dto.Quantity;
        _context.Shelves.Update(shelf);

        await _context.SaveChangesAsync();
        return stock;
    }
}