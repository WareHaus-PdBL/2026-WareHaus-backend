using WareHaus.API.Data;
using WareHaus.Api.DTOs.Zone;
using WareHaus.Api.DTOs.Shelf;
using WareHaus.API.Models;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using SkiaSharp;

namespace WareHaus.Api.Services;

class ZoneService : IZoneService
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ZoneService(AppDbContext context, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }

    // Get All Zones
    public async Task<List<GetAllZonesDto>> GetAllZonesAsync()
    {
        return await _context.Zones.Select(z => new GetAllZonesDto
        {
            Id = z.Id,
            ZoneCode = z.ZoneCode,
            ZoneName = z.ZoneName,
            Category = z.Category,
            TotalAisle = z.TotalAisle,
            ShelfPerAisle = z.ShelfPerAisle
        }).ToListAsync();
    }

    // Get Zone Details by Aisle
    public async Task<GetDetailsZoneDto> GetDetailsZoneAsync(int zoneId, int? aisle)
    {
        var zone = await _context.Zones.Where(z => z.Id == zoneId)
            .Select(z => new GetDetailsZoneDto
            {
                Id = z.Id,
                ZoneCode = z.ZoneCode,
                ZoneName = z.ZoneName,
                Category = z.Category,
                TotalAisle = z.TotalAisle,
                ShelfPerAisle = z.ShelfPerAisle,
                Shelves = z.Shelves.Where(s => s.Aisle == aisle).Select(s => new GetShelfForZoneDto
                {
                    Id = s.Id,
                    ShelfCode = s.ShelfCode,
                    Aisle = s.Aisle,
                    Capacity = s.Capacity,
                    CurrentVolume = s.CurrentVolume,
                    QRCodePath = s.QRCodePath ?? string.Empty
                }).ToList()
            }).FirstOrDefaultAsync();

        if (zone == null)
        {
            throw new KeyNotFoundException("Zone not found");
        }

        return zone;
    }

    // Create Zone
    public async Task<GetAllZonesDto> CreateZoneAsync(CreateZoneDto createZoneDto)
    {
        if (await _context.Zones.AnyAsync(z => z.ZoneCode == createZoneDto.ZoneCode))
        {
            throw new InvalidOperationException("Zona " + createZoneDto.ZoneCode + " sudah ada");
        }

        var zone = new Zone
        {
            ZoneCode = createZoneDto.ZoneCode,
            ZoneName = createZoneDto.ZoneName,
            Category = createZoneDto.Category,
            TotalAisle = createZoneDto.TotalAisle,
            ShelfPerAisle = createZoneDto.ShelfPerAisle,
            Description = createZoneDto.Description
        };

        for (int aisle = 1; aisle <= zone.TotalAisle; aisle++)
        {
            for (int shelfNum = 1; shelfNum <= zone.ShelfPerAisle; shelfNum++)
            {
                var shelf = new Shelf
                {
                    ShelfCode = $"{zone.ZoneCode}-{aisle}-{shelfNum}",
                    Aisle = aisle,
                    Capacity = createZoneDto.CapacityPerShelf,
                    CurrentVolume = 0,
                    QRCodePath = await GenerateQRCodeAsync($"{zone.ZoneCode}-{aisle}-{shelfNum}", zone.ZoneCode)
                };
                zone.Shelves.Add(shelf);
            }
        }

        _context.Zones.Add(zone);
        await _context.SaveChangesAsync();

        return new GetAllZonesDto
        {
            Id = zone.Id,
            ZoneCode = zone.ZoneCode,
            ZoneName = zone.ZoneName,
            Category = zone.Category,
            TotalAisle = zone.TotalAisle,
            ShelfPerAisle = zone.ShelfPerAisle
        };
    }

    // Update Zone
    public async Task<GetAllZonesDto> UpdateZoneAsync(int zoneId, UpdateZoneDto updateZoneDto)
    {
        var zone = await _context.Zones.FindAsync(zoneId);

        if (zone == null)
        {
            throw new KeyNotFoundException("Zona tidak ditemukan");
        }

        if (!string.IsNullOrEmpty(updateZoneDto.ZoneName))
            zone.ZoneName = updateZoneDto.ZoneName;

        if (!string.IsNullOrEmpty(updateZoneDto.Category))
            zone.Category = updateZoneDto.Category;

        if (!string.IsNullOrEmpty(updateZoneDto.Description))
            zone.Description = updateZoneDto.Description;

        await _context.SaveChangesAsync();

        return new GetAllZonesDto
        {
            Id = zone.Id,
            ZoneCode = zone.ZoneCode,
            ZoneName = zone.ZoneName,
            Category = zone.Category,
            TotalAisle = zone.TotalAisle,
            ShelfPerAisle = zone.ShelfPerAisle
        };
    }

    // Delete Zone
    public async Task DeleteZoneAsync(int zoneId)
    {
        var zone = await _context.Zones.Include(z => z.Shelves).FirstOrDefaultAsync(z => z.Id == zoneId);
        if (zone.Shelves.Any(s => s.CurrentVolume > 0))
        {
            throw new InvalidOperationException("Tidak dapat menghapus zona yang masih memiliki barang di rak.");
        }

        if (zone == null)
        {
            throw new KeyNotFoundException("Zone Tidak ditemukan");
        }

        _context.Zones.Remove(zone);
        await _context.SaveChangesAsync();
    }

    // Generate QR Code for Shelf
    private async Task<string> GenerateQRCodeAsync(string shelfCode, string zoneCode)
    {
        string folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "qrcodes", zoneCode);

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string fileName = $"{shelfCode}.png";
        string filePath = Path.Combine(folderPath, fileName);

        using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
        using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(shelfCode, QRCodeGenerator.ECCLevel.Q))
        using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
        {
            byte[] qrCodeImage = qrCode.GetGraphic(20);

            using (var ms = new MemoryStream(qrCodeImage))
            using (SKBitmap bitmap = SKBitmap.Decode(ms))
            {
                int textSpace = 80;
                var info = new SKImageInfo(bitmap.Width, bitmap.Height + textSpace);

                using (SKSurface surface = SKSurface.Create(info))
                {
                    SKCanvas canvas = surface.Canvas;
                    canvas.Clear(SKColors.White);
                    canvas.DrawBitmap(bitmap, 0, 0);

                    using (SKPaint paint = new SKPaint())
                    {
                        paint.Color = SKColors.Black;
                        paint.IsAntialias = true;
                        paint.TextSize = 32.0f;
                        paint.TextAlign = SKTextAlign.Center;
                        paint.Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);

                        float x = info.Width / 2;
                        float y = bitmap.Height + (textSpace / 2) + 10;

                        canvas.DrawText(shelfCode, x, y, paint);
                    }

                    using (SKImage image = surface.Snapshot())
                    using (SKData data = image.Encode(SKEncodedImageFormat.Png, 100))
                    using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        data.SaveTo(fileStream);
                    }
                }
            }

            return $"/qrcode/{zoneCode}/{fileName}"; // Placeholder, implementasi sebenarnya diperlukan
        }
    }
}