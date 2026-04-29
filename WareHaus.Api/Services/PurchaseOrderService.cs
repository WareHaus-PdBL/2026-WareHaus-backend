using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WareHaus.API.Data;
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
        var po = new PurchaseOrders
        {
            PONumber = dto.PONumber,
            SupplierName = dto.SupplierName,
            Status = "Pending"
        };

        foreach (var item in dto.Items)
        {
            po.POItems.Add(new POItems
            {
                ProductId = item.ProductId,
                QtyExpected = item.QtyExpected
            });
        }

        _context.PurchaseOrders.Add(po);
        await _context.SaveChangesAsync();

        return new PurchaseOrderResponseDto(
            po.Id, 
            po.PONumber, 
            po.SupplierName, 
            po.Status, 
            po.POItems.Select(i => new POItemResponseDto(i.Id, i.ProductId, i.QtyExpected)).ToList()
        );
    }
}