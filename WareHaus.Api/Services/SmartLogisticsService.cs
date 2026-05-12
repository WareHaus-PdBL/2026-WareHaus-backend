using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WareHaus.Api.Data;
using WareHaus.Api.DTOs;

namespace WareHaus.Api.Services;

public class SmartLogisticsService
{
    private readonly AppDbContext _context;

    public SmartLogisticsService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<PutAwayRecommendationDto>> GetPutAwayRecommendations(int productId)
    {
        return await _context.Shelves
            .Include(s => s.Zones)
            .Where(s => s.CurrentCapacity < s.MaxCapacity && s.DeletedAt == null)
            .OrderBy(s => s.CurrentCapacity) 
            .Select(s => new PutAwayRecommendationDto(
                s.Id,
                s.ShelfCode,
                s.Zones != null ? s.Zones.ZoneName : "No Zone",
                s.MaxCapacity - s.CurrentCapacity
            ))
            .Take(5)
            .ToListAsync();
    }

    public async Task<List<PickingRecommendationDto>> GetPickingRecommendations(int productId)
    {
        return await _context.ReceivingLogs
            .Include(r => r.POItems)
            .Where(r => r.POItems != null && r.POItems.ProductId == productId && r.QtyReceived > 0 && r.DeletedAt == null)
            .OrderBy(r => r.ExpiryDate) 
            .Select(r => new PickingRecommendationDto(
                r.Id, 
                "Area Inbound / Rak Utama", 
                r.QtyReceived,
                r.ExpiryDate
            ))
            .Take(5)
            .ToListAsync();
    }
}