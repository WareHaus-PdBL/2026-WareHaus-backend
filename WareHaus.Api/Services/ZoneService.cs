using WareHaus.Api.Data;
using WareHaus.Api.DTOs;
using WareHaus.Api.Models;
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

    public async Task<List<GetAllZonesDto>> GetAllZonesAsync()
    {
        return await _context.Zones
            .Select(z => new GetAllZonesDto(
                z.Id,
                z.ZoneCode,
                z.ZoneName,
                z.Category,
                z.Description,
                z.TotalAisle,
                z.ShelfPerAisle))
            .ToListAsync();
    }

    public async Task<GetDetailsZoneDto> GetDetailsZoneAsync(int zoneId, int? aisle)
{
    var zone = await _context.Zones
        .Where(z => z.Id == zoneId)
        .Select(z => new GetDetailsZoneDto(
            z.Id,
            z.ZoneCode,
            z.ZoneName,
            z.Category,
            z.Description,
            z.TotalAisle,
            z.ShelfPerAisle,
            z.Shelves.Count(s => s.CurrentVolume <= 0),
            null,
            z.Shelves
                .Where(s => aisle == null || s.Aisle == aisle)
                .Select(s => new GetShelfForZoneDto(
                    s.Id,
                    s.ShelfCode,
                    s.Aisle,
                    s.Capacity,
                    s.CurrentVolume,
                    s.QRCodePath ?? string.Empty))
                .ToList()
        ))
        .FirstOrDefaultAsync();

    if (zone == null)
    {
        throw new KeyNotFoundException("Zone not found");
    }

    var shelves = zone.Shelves ?? new List<GetShelfForZoneDto>();

    if (aisle.HasValue)
    {
        return new GetDetailsZoneDto(
            zone.Id,
            zone.ZoneCode,
            zone.ZoneName,
            zone.Category,
            zone.Description,
            zone.TotalAisle,
            zone.ShelfPerAisle,
            zone.EmptyShelves,
            null,
            shelves);
    }

    var aisles = Enumerable.Range(1, zone.TotalAisle)
        .Select(i => new GetAisleDto(
            i,
            !shelves.Any(s => s.Aisle == i && s.CurrentVolume > 0),
            shelves.Count(s => s.Aisle == i),
            shelves.Where(s => s.Aisle == i).Sum(s => s.Capacity),
            shelves.Where(s => s.Aisle == i).Sum(s => s.CurrentVolume)))
        .ToList();

    return new GetDetailsZoneDto(
        zone.Id,
        zone.ZoneCode,
        zone.ZoneName,
        zone.Category,
        zone.Description,
        zone.TotalAisle,
        zone.ShelfPerAisle,
        zone.EmptyShelves,
        aisles,
        shelves);
}

    public async Task<GetAllZonesDto> CreateZoneAsync(CreateZoneDto createZoneDto)
    {
        if (await _context.Zones.AnyAsync(z => z.ZoneCode == createZoneDto.ZoneCode))
        {
            throw new InvalidOperationException("Zona " + createZoneDto.ZoneCode + " sudah ada");
        }

        var zone = new Zones
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
                var shelf = new Shelves
                {
                    ShelfCode = $"{zone.ZoneCode}-{aisle}-{shelfNum}",
                    Aisle = aisle,
                    Capacity = createZoneDto.CapacityPerShelf,
                    CurrentVolume = 0,
                    MaxCapacity = createZoneDto.CapacityPerShelf,
                    CurrentCapacity = 0,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    QRCodePath = await GenerateQRCodeAsync($"{zone.ZoneCode}-{aisle}-{shelfNum}", zone.ZoneCode)
                };

                zone.Shelves.Add(shelf);
            }
        }

        _context.Zones.Add(zone);
        await _context.SaveChangesAsync();

        return new GetAllZonesDto(
            zone.Id,
            zone.ZoneCode,
            zone.ZoneName,
            zone.Category,
            zone.Description,
            zone.TotalAisle,
            zone.ShelfPerAisle);
    }

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

        return new GetAllZonesDto(
            zone.Id,
            zone.ZoneCode,
            zone.ZoneName,
            zone.Category,
            zone.Description,
            zone.TotalAisle,
            zone.ShelfPerAisle);
    }

public async Task<byte[]> DownloadAisleQrCodesPdfAsync(int zoneId, int aisle)
{
    var zone = await _context.Zones
        .Include(z => z.Shelves)
        .FirstOrDefaultAsync(z => z.Id == zoneId);

    if (zone == null)
    {
        throw new KeyNotFoundException("Zone not found");
    }

    var shelves = zone.Shelves
        .Where(s => s.Aisle == aisle)
        .OrderBy(s => s.ShelfCode)
        .ToList();

    if (!shelves.Any())
    {
        throw new KeyNotFoundException("Shelf untuk aisle ini tidak ditemukan");
    }

    var webRootPath = _webHostEnvironment.WebRootPath;

    if (string.IsNullOrWhiteSpace(webRootPath))
    {
        webRootPath = Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot");
    }

    using var memoryStream = new MemoryStream();
    using var document = SKDocument.CreatePdf(memoryStream);

    const float pageWidth = 595;
    const float pageHeight = 842;

    var canvas = document.BeginPage(pageWidth, pageHeight);
    canvas.Clear(SKColors.White);

    using var titlePaint = new SKPaint
    {
        Color = SKColors.Black,
        IsAntialias = true
    };

    using var titleFont = new SKFont(
        SKTypeface.FromFamilyName(
            "Arial",
            SKFontStyleWeight.Bold,
            SKFontStyleWidth.Normal,
            SKFontStyleSlant.Upright),
        24);

    canvas.DrawText(
        $"QR Codes - Zone {zone.ZoneCode} Aisle {aisle}",
        pageWidth / 2,
        50,
        SKTextAlign.Center,
        titleFont,
        titlePaint);

    using var textPaint = new SKPaint
    {
        Color = SKColors.Black,
        IsAntialias = true
    };

    using var textFont = new SKFont(
        SKTypeface.FromFamilyName("Arial"),
        16);

    float startX = 70;
    float startY = 100;
    float qrSize = 150;
    float gapX = 80;
    float gapY = 90;

    int column = 0;
    int row = 0;
    int maxColumns = 2;
    int maxRows = 3;

    foreach (var shelf in shelves)
    {
        if (row >= maxRows)
        {
            document.EndPage();

            canvas = document.BeginPage(pageWidth, pageHeight);
            canvas.Clear(SKColors.White);

            canvas.DrawText(
                $"QR Codes - Zone {zone.ZoneCode} Aisle {aisle}",
                pageWidth / 2,
                50,
                SKTextAlign.Center,
                titleFont,
                titlePaint);

            column = 0;
            row = 0;
        }

        float x = startX + column * (qrSize + gapX);
        float y = startY + row * (qrSize + gapY);

        if (!string.IsNullOrWhiteSpace(shelf.QRCodePath))
        {
            var relativePath = shelf.QRCodePath
                .TrimStart('/')
                .Replace("/", Path.DirectorySeparatorChar.ToString());

            var qrPath = Path.Combine(webRootPath, relativePath);

            if (File.Exists(qrPath))
            {
                using var bitmap = SKBitmap.Decode(qrPath);

                var destination = new SKRect(x, y, x + qrSize, y + qrSize);
                canvas.DrawBitmap(bitmap, destination);
            }
            else
            {
                canvas.DrawText(
                    "QR not found",
                    x + qrSize / 2,
                    y + qrSize / 2,
                    SKTextAlign.Center,
                    textFont,
                    textPaint);
            }
        }

        canvas.DrawText(
            shelf.ShelfCode,
            x + qrSize / 2,
            y + qrSize + 25,
            SKTextAlign.Center,
            textFont,
            textPaint);

        column++;

        if (column >= maxColumns)
        {
            column = 0;
            row++;
        }
    }

    document.EndPage();
    document.Close();

    return memoryStream.ToArray();
}
    public async Task DeleteZoneAsync(int zoneId)
    {
        var zone = await _context.Zones
            .Include(z => z.Shelves)
            .FirstOrDefaultAsync(z => z.Id == zoneId);

        if (zone == null)
        {
            throw new KeyNotFoundException("Zone Tidak ditemukan");
        }

        if (zone.Shelves.Any(s => s.CurrentVolume > 0))
        {
            throw new InvalidOperationException("Tidak dapat menghapus zona yang masih memiliki barang di rak.");
        }

        string folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "qrcodes", zone.ZoneCode);

        if (Directory.Exists(folderPath))
        {
            Directory.Delete(folderPath, true);
        }

        _context.Zones.Remove(zone);
        await _context.SaveChangesAsync();
    }

    private async Task<string> GenerateQRCodeAsync(string shelfCode, string zoneCode)
    {
        var webRootPath = _webHostEnvironment.WebRootPath;
        if (string.IsNullOrWhiteSpace(webRootPath))
        {
            webRootPath = Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot");
        }

        string folderPath = Path.Combine(webRootPath, "qrcodes", zoneCode);
        Directory.CreateDirectory(folderPath);

        string fileName = $"{shelfCode}.png";
        string filePath = Path.Combine(folderPath, fileName);

        using QRCodeGenerator qrGenerator = new QRCodeGenerator();
        using QRCodeData qrCodeData = qrGenerator.CreateQrCode(shelfCode, QRCodeGenerator.ECCLevel.Q);
        using PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);

        byte[] qrCodeImage = qrCode.GetGraphic(20);

        using var ms = new MemoryStream(qrCodeImage);
        using SKBitmap bitmap = SKBitmap.Decode(ms);

        int textSpace = 80;
        var info = new SKImageInfo(bitmap.Width, bitmap.Height + textSpace);

        using SKSurface surface = SKSurface.Create(info);
        SKCanvas canvas = surface.Canvas;
        canvas.Clear(SKColors.White);
        canvas.DrawBitmap(bitmap, 0, 0);

        using SKPaint paint = new SKPaint();
        using SKFont font = new SKFont(
            SKTypeface.FromFamilyName("Arial", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright),
            32);

        paint.Color = SKColors.Black;
        paint.IsAntialias = true;

        float x = info.Width / 2;
        float y = bitmap.Height + (textSpace / 2) + 10;

        canvas.DrawText(shelfCode, x, y, SKTextAlign.Center, font, paint);

        using SKImage image = surface.Snapshot();
        using SKData data = image.Encode(SKEncodedImageFormat.Png, 100);
        using FileStream fileStream = new FileStream(filePath, FileMode.Create);

        data.SaveTo(fileStream);

        await Task.CompletedTask;
        return $"/qrcodes/{zoneCode}/{fileName}";
    }

    public async Task<GetShelfForZoneDetailsDto> GetShelfDetailsAsync(int shelfId)
    {
        var shelf = await _context.Shelves
            .Include(s => s.Stocks)
            .ThenInclude(st => st.Products)
            .Where(s => s.Id == shelfId)
            .Select(s => new GetShelfForZoneDetailsDto(
                s.Id,
                s.ShelfCode,
                s.Aisle,
                s.Capacity,
                s.CurrentVolume,
                s.QRCodePath ?? string.Empty,
                s.Stocks.Where(st => st.Products != null).Select(st => new GetStocksInShelfDto(
                    st.Id,
                    st.ShelfId,
                    st.ProductId,
                    st.Quantity,
                    new GetProductDto(
                        st.Products!.Id,
                        st.Products!.SKU,
                        st.Products!.ProductName,
                        st.Products!.Barcode,
                        st.Products!.UnitOfMeasure,
                        st.Quantity
                    )
                )).ToList()
            ))
            .FirstOrDefaultAsync();

        if (shelf == null)
        {
            throw new KeyNotFoundException("Rak tidak ditemukan");
        }

        return shelf;
    }

    public async Task<List<GetShelfForZoneDto>> GetShelvesInZoneAsync(int zoneId)
    {
        var zone = await _context.Zones.FindAsync(zoneId);

        if (zone == null)
        {
            throw new KeyNotFoundException("Zone not found");
        }

        return await _context.Shelves
            .Where(s => s.ZoneId == zoneId)
            .Select(s => new GetShelfForZoneDto(
                s.Id,
                s.ShelfCode,
                s.Aisle,
                s.Capacity,
                s.CurrentVolume,
                s.QRCodePath ?? string.Empty))
            .ToListAsync();
    }

 public async Task<DownloadQrFileDto> DownloadQRCodeShelfAsync(int shelfId, string option)
{
    option = option?.Trim().ToLowerInvariant() ?? string.Empty;

    if (option != "png" && option != "pdf")
    {
        throw new InvalidOperationException("Option harus 'png' atau 'pdf'");
    }

    var shelf = await _context.Shelves.FindAsync(shelfId);

    if (shelf == null)
    {
        throw new KeyNotFoundException("Rak tidak ditemukan");
    }

    var zone = await _context.Zones.FindAsync(shelf.ZoneId);

    if (zone == null)
    {
        throw new KeyNotFoundException("Zone tidak ditemukan");
    }

    var webRootPath = _webHostEnvironment.WebRootPath;

    if (string.IsNullOrWhiteSpace(webRootPath))
    {
        webRootPath = Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot");
    }

    var qrFilePath = Path.Combine(
        webRootPath,
        "qrcodes",
        zone.ZoneCode,
        $"{shelf.ShelfCode}.png"
    );

    if (!File.Exists(qrFilePath))
    {
        throw new FileNotFoundException($"File QR Code tidak ditemukan: {qrFilePath}");
    }

    if (option == "png")
    {
        var pngBytes = await File.ReadAllBytesAsync(qrFilePath);

        return new DownloadQrFileDto(
            pngBytes,
            "image/png",
            $"{shelf.ShelfCode}.png"
        );
    }

    var pdfBytes = CreateShelfQrPdf(qrFilePath, shelf.ShelfCode, zone.ZoneCode);

    return new DownloadQrFileDto(
        pdfBytes,
        "application/pdf",
        $"{shelf.ShelfCode}.pdf"
    );
}

private byte[] CreateShelfQrPdf(string qrFilePath, string shelfCode, string zoneCode)
{
    using var memoryStream = new MemoryStream();
    using var document = SKDocument.CreatePdf(memoryStream);

    const float pageWidth = 595;
    const float pageHeight = 842;

    var canvas = document.BeginPage(pageWidth, pageHeight);
    canvas.Clear(SKColors.White);

    using var paint = new SKPaint
    {
        Color = SKColors.Black,
        IsAntialias = true
    };

    using var titleFont = new SKFont(
        SKTypeface.FromFamilyName(
            "Arial",
            SKFontStyleWeight.Bold,
            SKFontStyleWidth.Normal,
            SKFontStyleSlant.Upright),
        28);

    using var normalFont = new SKFont(
        SKTypeface.FromFamilyName("Arial"),
        18);

    canvas.DrawText(
        $"QR Code - {shelfCode}",
        pageWidth / 2,
        80,
        SKTextAlign.Center,
        titleFont,
        paint);

    canvas.DrawText(
        $"Zone {zoneCode}",
        pageWidth / 2,
        115,
        SKTextAlign.Center,
        normalFont,
        paint);

    using var bitmap = SKBitmap.Decode(qrFilePath);

    if (bitmap == null)
    {
        throw new InvalidOperationException("QR Code gagal dibaca");
    }

    const float qrSize = 350;
    float x = (pageWidth - qrSize) / 2;
    float y = 180;

    var destination = new SKRect(x, y, x + qrSize, y + qrSize);
    canvas.DrawBitmap(bitmap, destination);

    canvas.DrawText(
        shelfCode,
        pageWidth / 2,
        y + qrSize + 50,
        SKTextAlign.Center,
        titleFont,
        paint);

    document.EndPage();
    document.Close();

    return memoryStream.ToArray();
}
public async Task<DownloadQrFileDto> DownloadQRCodeAisleAsync(int zoneId, int aisle, string option)
{
    option = option?.Trim().ToLowerInvariant() ?? string.Empty;

    if (option != "png" && option != "pdf")
    {
        throw new InvalidOperationException("Option harus 'png' atau 'pdf'");
    }

    var zone = await _context.Zones
        .Include(z => z.Shelves)
        .FirstOrDefaultAsync(z => z.Id == zoneId);

    if (zone == null)
    {
        throw new KeyNotFoundException("Zone tidak ditemukan");
    }

    var shelves = zone.Shelves
        .Where(s => s.Aisle == aisle)
        .OrderBy(s => s.ShelfCode)
        .ToList();

    if (!shelves.Any())
    {
        throw new KeyNotFoundException("Shelf untuk aisle ini tidak ditemukan");
    }

    var webRootPath = _webHostEnvironment.WebRootPath;

    if (string.IsNullOrWhiteSpace(webRootPath))
    {
        webRootPath = Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot");
    }

    var qrItems = shelves.Select(shelf =>
    {
        var qrFilePath = Path.Combine(
            webRootPath,
            "qrcodes",
            zone.ZoneCode,
            $"{shelf.ShelfCode}.png"
        );

        if (!File.Exists(qrFilePath))
        {
            throw new FileNotFoundException($"File QR Code tidak ditemukan: {qrFilePath}");
        }

        return new AisleQrItem(
            shelf.ShelfCode,
            qrFilePath
        );
    }).ToList();

    if (option == "pdf")
    {
        var pdfBytes = CreateAisleQrPdf(qrItems, zone.ZoneCode, aisle);

        return new DownloadQrFileDto(
            pdfBytes,
            "application/pdf",
            $"zone-{zone.ZoneCode}-aisle-{aisle}-qrcodes.pdf"
        );
    }

    var pngBytes = CreateAisleQrPng(qrItems, zone.ZoneCode, aisle);

    return new DownloadQrFileDto(
        pngBytes,
        "image/png",
        $"zone-{zone.ZoneCode}-aisle-{aisle}-qrcodes.png"
    );
}

private byte[] CreateAisleQrPdf(List<AisleQrItem> qrItems, string zoneCode, int aisle)
{
    using var memoryStream = new MemoryStream();
    using var document = SKDocument.CreatePdf(memoryStream);

    const float pageWidth = 595;
    const float pageHeight = 842;

    using var paint = new SKPaint
    {
        Color = SKColors.Black,
        IsAntialias = true
    };

    using var titleFont = new SKFont(
        SKTypeface.FromFamilyName(
            "Arial",
            SKFontStyleWeight.Bold,
            SKFontStyleWidth.Normal,
            SKFontStyleSlant.Upright),
        28);

    using var normalFont = new SKFont(
        SKTypeface.FromFamilyName("Arial"),
        18);

    foreach (var item in qrItems)
    {
        var canvas = document.BeginPage(pageWidth, pageHeight);
        canvas.Clear(SKColors.White);

        canvas.DrawText(
            $"QR Code - {item.ShelfCode}",
            pageWidth / 2,
            80,
            SKTextAlign.Center,
            titleFont,
            paint);

        canvas.DrawText(
            $"Zone {zoneCode} - Aisle {aisle}",
            pageWidth / 2,
            115,
            SKTextAlign.Center,
            normalFont,
            paint);

        using var qrBitmap = SKBitmap.Decode(item.QrFilePath);

        if (qrBitmap == null)
        {
            throw new InvalidOperationException($"QR Code gagal dibaca: {item.ShelfCode}");
        }

        const float qrSize = 350;
        float x = (pageWidth - qrSize) / 2;
        float y = 180;

        var destination = new SKRect(x, y, x + qrSize, y + qrSize);
        canvas.DrawBitmap(qrBitmap, destination);

        canvas.DrawText(
            item.ShelfCode,
            pageWidth / 2,
            y + qrSize + 50,
            SKTextAlign.Center,
            titleFont,
            paint);

        document.EndPage();
    }

    document.Close();

    return memoryStream.ToArray();
}

private byte[] CreateAisleQrPng(List<AisleQrItem> qrItems, string zoneCode, int aisle)
{
    const int maxColumns = 2;

    const int qrSize = 220;
    const int startX = 70;
    const int startY = 120;
    const int gapX = 80;
    const int gapY = 90;

    int totalRows = (int)Math.Ceiling(qrItems.Count / (double)maxColumns);

    int imageWidth = startX * 2 + (maxColumns * qrSize) + ((maxColumns - 1) * gapX);
    int imageHeight = startY + (totalRows * qrSize) + ((totalRows - 1) * gapY) + 100;

    using var bitmap = new SKBitmap(imageWidth, imageHeight);
    using var canvas = new SKCanvas(bitmap);

    canvas.Clear(SKColors.White);

    using var paint = new SKPaint
    {
        Color = SKColors.Black,
        IsAntialias = true
    };

    using var titleFont = new SKFont(
        SKTypeface.FromFamilyName(
            "Arial",
            SKFontStyleWeight.Bold,
            SKFontStyleWidth.Normal,
            SKFontStyleSlant.Upright),
        28);

    using var textFont = new SKFont(
        SKTypeface.FromFamilyName("Arial"),
        18);

    canvas.DrawText(
        $"QR Codes - Zone {zoneCode} Aisle {aisle}",
        imageWidth / 2,
        60,
        SKTextAlign.Center,
        titleFont,
        paint);

    for (int index = 0; index < qrItems.Count; index++)
    {
        int column = index % maxColumns;
        int row = index / maxColumns;

        float x = startX + column * (qrSize + gapX);
        float y = startY + row * (qrSize + gapY);

        DrawQrItem(canvas, qrItems[index], x, y, qrSize, textFont, paint);
    }

    using var image = SKImage.FromBitmap(bitmap);
    using var data = image.Encode(SKEncodedImageFormat.Png, 100);

    return data.ToArray();
}

private void DrawQrItem(
    SKCanvas canvas,
    AisleQrItem item,
    float x,
    float y,
    float qrSize,
    SKFont textFont,
    SKPaint paint)
{
    using var qrBitmap = SKBitmap.Decode(item.QrFilePath);

    if (qrBitmap == null)
    {
        throw new InvalidOperationException($"QR Code gagal dibaca: {item.ShelfCode}");
    }

    var destination = new SKRect(x, y, x + qrSize, y + qrSize);
    canvas.DrawBitmap(qrBitmap, destination);

    canvas.DrawText(
        item.ShelfCode,
        x + qrSize / 2,
        y + qrSize + 25,
        SKTextAlign.Center,
        textFont,
        paint);
}
}