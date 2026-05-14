using Microsoft.EntityFrameworkCore;
using WareHaus.Api.Data;
using WareHaus.Api.DTOs;

namespace WareHaus.Api.Services;

public class DashboardService
{
    private readonly AppDbContext _context;

    public DashboardService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<RecentStockLogDto>> GetRecentStockLogsAsync(int limit = 10)
    {
        if (limit <= 0)
        {
            limit = 10;
        }

        var logs = await _context.StockLogs
            .AsNoTracking()
            .Where(log => log.DeletedAt == null)
            .OrderByDescending(log => log.CreatedAt)
            .Take(limit)
            .Select(log => new
            {
                log.Id,
                log.MovementType,
                log.Quantity,
                log.StockAfterMovement,
                log.CreatedAt,

                ProductName = log.Products != null
                    ? log.Products.ProductName
                    : "Unknown Product",

                SKU = log.Products != null
                    ? log.Products.SKU
                    : string.Empty,

                ZoneName = log.Shelves != null && log.Shelves.Zones != null
                    ? log.Shelves.Zones.ZoneName
                    : "Unknown Zone",

                ShelfCode = log.Shelves != null
                    ? log.Shelves.ShelfCode
                    : "Unknown Shelf"
            })
            .ToListAsync();

        return logs.Select(log =>
        {
            var type = log.MovementType == "StockIn"
                ? "Stock In"
                : "Stock Out";

            var title = type;

            var locationName = $"{log.ZoneName} - {log.ShelfCode}";

            return new RecentStockLogDto(
                log.Id,
                type,
                title,
                log.ProductName,
                log.SKU,
                locationName,
                log.Quantity,
                log.StockAfterMovement,
                log.CreatedAt.ToLocalTime().ToString("HH:mm"),
                log.CreatedAt
            );
        }).ToList();
    }
}