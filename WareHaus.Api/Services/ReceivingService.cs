using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WareHaus.Api.Data;
using WareHaus.Api.Models;
using WareHaus.Api.DTOs;

namespace WareHaus.Api.Services;

public class ReceivingService
{
    private readonly AppDbContext _context;

    public ReceivingService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ReceivingResponseDto> ReceiveItemAsync(CreateReceivingDto dto)
    {
        var poItem = await _context.POItems
            .Include(pi => pi.PurchaseOrders)
            .FirstOrDefaultAsync(pi => pi.Id == dto.POItemId && pi.DeletedAt == null);

        if (poItem == null)
        {
            throw new KeyNotFoundException("PO Item not found");
        }

        var log = new ReceivingLogs
        {
            POItemId = dto.POItemId,
            QtyReceived = dto.QtyReceived,
            Condition = dto.Condition,
            ReceivedAt = DateTime.UtcNow,
            ExpiryDate = dto.ExpiryDate.ToUniversalTime(),
            PhotoUrl = dto.PhotoUrl ?? string.Empty,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.ReceivingLogs.Add(log);
        await _context.SaveChangesAsync();

        if (poItem.PurchaseOrders != null)
        {
            var allPoItems = await _context.POItems
                .Include(pi => pi.ReceivingLogs)
                .Where(pi => pi.POId == poItem.POId && pi.DeletedAt == null)
                .ToListAsync();

            bool allFulfilled = allPoItems.All(pi => pi.ReceivingLogs.Where(r => r.DeletedAt == null).Sum(r => r.QtyReceived) >= pi.QtyExpected);
            bool someFulfilled = allPoItems.Any(pi => pi.ReceivingLogs.Where(r => r.DeletedAt == null).Sum(r => r.QtyReceived) > 0);

            poItem.PurchaseOrders.Status = allFulfilled ? "Success" : (someFulfilled ? "Partial" : "Pending");
            poItem.PurchaseOrders.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return new ReceivingResponseDto(
            log.Id,
            log.POItemId,
            log.QtyReceived,
            log.Condition,
            log.ReceivedAt,
            log.ExpiryDate,
            log.PhotoUrl
        );
    }

    public async Task<List<ReceivingResponseDto>> GetAllReceivingLogsAsync()
    {
        return await _context.ReceivingLogs
            .Where(log => log.DeletedAt == null)
            .Select(log => new ReceivingResponseDto(
                log.Id,
                log.POItemId,
                log.QtyReceived,
                log.Condition,
                log.ReceivedAt,
                log.ExpiryDate,
                log.PhotoUrl
            ))
            .ToListAsync();
    }

    public async Task<List<ReceivingResponseDto>> GetLogsByPOItemAsync(int poItemId)
    {
        return await _context.ReceivingLogs
            .Where(log => log.POItemId == poItemId && log.DeletedAt == null)
            .Select(log => new ReceivingResponseDto(
                log.Id,
                log.POItemId,
                log.QtyReceived,
                log.Condition,
                log.ReceivedAt,
                log.ExpiryDate,
                log.PhotoUrl
            ))
            .ToListAsync();
    }
}