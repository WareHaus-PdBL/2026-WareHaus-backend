using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WareHaus.Api.Data;
using WareHaus.Api.Models;
using WareHaus.Api.DTOs;

namespace WareHaus.Api.Services;

public class PurchaseOrderService
{
    private readonly AppDbContext _context;

    public PurchaseOrderService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PurchaseOrderResponseDto> CreatePOAsync(CreatePurchaseOrderDto dto)
    {
        var finalPoNumber = string.IsNullOrWhiteSpace(dto.PONumber) 
            ? $"PO-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}" 
            : dto.PONumber;

        var po = new PurchaseOrders
        {
            PONumber = finalPoNumber,
            SupplierName = dto.SupplierName,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        foreach (var item in dto.Items)
        {
            po.POItems.Add(new POItems
            {
                ProductId = item.ProductId,
                QtyExpected = item.QtyExpected,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }

        _context.PurchaseOrders.Add(po);
        await _context.SaveChangesAsync();

        int totalExpected = po.POItems.Sum(i => i.QtyExpected);

        return new PurchaseOrderResponseDto(
            po.Id,
            po.PONumber,
            po.SupplierName,
            po.Status,
            totalExpected,
            0,
            po.POItems.Select(i => new POItemResponseDto(i.Id, i.ProductId, i.QtyExpected, 0)).ToList()
        );
    }

    public async Task<List<PurchaseOrderResponseDto>> GetAllPOsAsync()
    {
        return await _context.PurchaseOrders
            .Where(po => po.DeletedAt == null)
            .Include(po => po.POItems.Where(i => i.DeletedAt == null))
                .ThenInclude(i => i.ReceivingLogs.Where(r => r.DeletedAt == null))
            .Select(po => new PurchaseOrderResponseDto(
                po.Id,
                po.PONumber,
                po.SupplierName,
                po.Status,
                po.POItems.Sum(i => i.QtyExpected),
                po.POItems.Sum(i => i.ReceivingLogs.Sum(r => r.QtyReceived)),
                po.POItems.Select(i => new POItemResponseDto(
                    i.Id, 
                    i.ProductId, 
                    i.QtyExpected, 
                    i.ReceivingLogs.Sum(r => r.QtyReceived)
                )).ToList()
            ))
            .ToListAsync();
    }

    public async Task<PurchaseOrderResponseDto?> GetPODetailsAsync(int poId)
    {
        var po = await _context.PurchaseOrders
            .Where(p => p.DeletedAt == null && p.Id == poId)
            .Include(p => p.POItems.Where(i => i.DeletedAt == null))
                .ThenInclude(i => i.ReceivingLogs.Where(r => r.DeletedAt == null))
            .FirstOrDefaultAsync();

        if (po == null) return null;

        return new PurchaseOrderResponseDto(
            po.Id,
            po.PONumber,
            po.SupplierName,
            po.Status,
            po.POItems.Sum(i => i.QtyExpected),
            po.POItems.Sum(i => i.ReceivingLogs.Sum(r => r.QtyReceived)),
            po.POItems.Select(i => new POItemResponseDto(
                i.Id, 
                i.ProductId, 
                i.QtyExpected, 
                i.ReceivingLogs.Sum(r => r.QtyReceived)
            )).ToList()
        );
    }

    public async Task<PurchaseOrderResponseDto?> UpdatePOStatusAsync(int poId, string newStatus)
    {
        var po = await _context.PurchaseOrders
            .Where(p => p.DeletedAt == null && p.Id == poId)
            .Include(p => p.POItems.Where(i => i.DeletedAt == null))
                .ThenInclude(i => i.ReceivingLogs.Where(r => r.DeletedAt == null))
            .FirstOrDefaultAsync();

        if (po == null) return null;

        po.Status = newStatus;
        po.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();

        return new PurchaseOrderResponseDto(
            po.Id,
            po.PONumber,
            po.SupplierName,
            po.Status,
            po.POItems.Sum(i => i.QtyExpected),
            po.POItems.Sum(i => i.ReceivingLogs.Sum(r => r.QtyReceived)),
            po.POItems.Select(i => new POItemResponseDto(
                i.Id, 
                i.ProductId, 
                i.QtyExpected, 
                i.ReceivingLogs.Sum(r => r.QtyReceived)
            )).ToList()
        );
    }

    public async Task<bool> DeletePOAsync(int poId)
    {
        var po = await _context.PurchaseOrders
            .Include(po => po.POItems)
            .FirstOrDefaultAsync(p => p.Id == poId && p.DeletedAt == null);

        if (po == null) return false;

        po.DeletedAt = DateTime.UtcNow;
        foreach (var item in po.POItems)
        {
            item.DeletedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return true;
    }
}