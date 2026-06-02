using Microsoft.EntityFrameworkCore;
using WareHaus.Api.Data;
using WareHaus.Api.DTOs;
using WareHaus.Api.Models;

namespace WareHaus.Api.Services;

public class OutboundService
{
    private readonly AppDbContext _context;

    public OutboundService(AppDbContext context)
    {
        _context = context;
    }

    // ==========================
    // SALES ORDER
    // ==========================

    public async Task<List<SalesOrderResponseDto>> GetSalesOrdersAsync()
    {
        var salesOrders = await _context.SalesOrders
            .Include(order => order.SOItems)
                .ThenInclude(item => item.Product)
            .OrderBy(order => order.Id)
            .ToListAsync();

        return salesOrders.Select(MapSalesOrderResponse).ToList();
    }

    public async Task<SalesOrderResponseDto> GetSalesOrderByIdAsync(int id)
    {
        var salesOrder = await _context.SalesOrders
            .Include(order => order.SOItems)
                .ThenInclude(item => item.Product)
            .FirstOrDefaultAsync(order => order.Id == id);

        if (salesOrder == null)
        {
            throw new KeyNotFoundException("Sales Order tidak ditemukan.");
        }

        return MapSalesOrderResponse(salesOrder);
    }

    public async Task<SalesOrderResponseDto> CreateSalesOrderAsync(CreateSalesOrderDto dto)
    {
        if (dto.Items == null || !dto.Items.Any())
        {
            throw new InvalidOperationException("Sales Order harus memiliki minimal satu item.");
        }

        var salesOrder = new SalesOrders
        {
            SONumber = GenerateNumber("SO"),
            CustomerName = dto.CustomerName,
            ShippingAddress = dto.ShippingAddress,
            Courier = dto.Courier,
            RequiredDeliveryDate = dto.RequiredDeliveryDate,
            TrackingNumber = null,
            Status = OutboundStatus.Queued,
            OrderDate = DateTime.UtcNow
        };

        foreach (var item in dto.Items)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(product => product.Id == item.ProductId);

            if (product == null)
            {
                throw new KeyNotFoundException($"Product dengan ID {item.ProductId} tidak ditemukan.");
            }

            var availableStock = await _context.Stocks
                .Where(stock => stock.ProductId == item.ProductId)
                .SumAsync(stock => stock.Quantity);

            if (availableStock <= 0)
            {
                throw new InvalidOperationException(
                    $"Stock product {product.ProductName} kosong.");
            }

            if (item.QtyOrdered > availableStock)
            {
                throw new InvalidOperationException(
                    $"Qty product {product.ProductName} melebihi stock tersedia. Stock tersedia: {availableStock}.");
            }

            salesOrder.SOItems.Add(new SOItems
            {
                ProductId = item.ProductId,
                QtyOrdered = item.QtyOrdered,
                QtyPicked = 0,
                QtyVerified = 0,
                UnitOfMeasureSnapshot = product.UnitOfMeasure
            });
        }

        _context.SalesOrders.Add(salesOrder);
        await _context.SaveChangesAsync();

        return await GetSalesOrderByIdAsync(salesOrder.Id);
    }

    public async Task<SalesOrderResponseDto> UpdateSalesOrderAsync(int id, UpdateSalesOrderDto dto)
    {
        var salesOrder = await _context.SalesOrders
            .Include(order => order.SOItems)
                .ThenInclude(item => item.Product)
            .FirstOrDefaultAsync(order => order.Id == id);

        if (salesOrder == null)
        {
            throw new KeyNotFoundException("Sales Order tidak ditemukan.");
        }

        if (!string.IsNullOrWhiteSpace(dto.CustomerName))
        {
            salesOrder.CustomerName = dto.CustomerName;
        }

        if (!string.IsNullOrWhiteSpace(dto.ShippingAddress))
        {
            salesOrder.ShippingAddress = dto.ShippingAddress;
        }

        if (!string.IsNullOrWhiteSpace(dto.Courier))
        {
            salesOrder.Courier = dto.Courier;
        }

        if (dto.RequiredDeliveryDate.HasValue)
        {
            salesOrder.RequiredDeliveryDate = dto.RequiredDeliveryDate.Value;
        }

        if (!string.IsNullOrWhiteSpace(dto.TrackingNumber))
        {
            salesOrder.TrackingNumber = dto.TrackingNumber;
        }

        if (!string.IsNullOrWhiteSpace(dto.Status))
        {
            ValidateOutboundStatus(dto.Status);
            salesOrder.Status = dto.Status;
        }

        await _context.SaveChangesAsync();

        return MapSalesOrderResponse(salesOrder);
    }

    public async Task DeleteSalesOrderAsync(int id)
    {
        var salesOrder = await _context.SalesOrders
            .FirstOrDefaultAsync(order => order.Id == id);

        if (salesOrder == null)
        {
            throw new KeyNotFoundException("Sales Order tidak ditemukan.");
        }

        salesOrder.DeletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    // ==========================
    // PICKING
    // ==========================

    public async Task<PickingTaskResponseDto> CreatePickingTaskAsync(CreatePickingTaskDto dto)
    {
        var salesOrder = await _context.SalesOrders
            .Include(order => order.SOItems)
                .ThenInclude(item => item.Product)
            .FirstOrDefaultAsync(order => order.Id == dto.SalesOrderId);

        if (salesOrder == null)
        {
            throw new KeyNotFoundException("Sales Order tidak ditemukan.");
        }

        if (!salesOrder.SOItems.Any())
        {
            throw new InvalidOperationException("Sales Order belum memiliki item.");
        }

        var existingPickingTask = await _context.PickingTasks
            .AnyAsync(task => task.SalesOrderId == salesOrder.Id);

        if (existingPickingTask)
        {
            throw new InvalidOperationException("Sales Order ini sudah memiliki Picking Task.");
        }

        var pickingTask = new PickingTasks
        {
            PickingNumber = GenerateNumber("PICK"),
            SalesOrderId = salesOrder.Id,
            StartTime = DateTime.UtcNow,
            EndTime = null,
            TotalItems = salesOrder.SOItems.Sum(item => item.QtyOrdered),
            PickingStatus = OutboundStatus.Active
        };

        foreach (var soItem in salesOrder.SOItems)
        {
            var remainingQty = soItem.QtyOrdered;

            var stocks = await _context.Stocks
                .Include(stock => stock.Shelves!)
                    .ThenInclude(shelf => shelf.Zones)
                .Where(stock =>
                    stock.ProductId == soItem.ProductId &&
                    stock.Quantity > 0)
                .OrderBy(stock => stock.ShelfId)
                .ToListAsync();

            foreach (var stock in stocks)
            {
                if (remainingQty <= 0)
                {
                    break;
                }

                var qtyToPick = Math.Min(remainingQty, stock.Quantity);

                pickingTask.PickingItems.Add(new PickingItems
                {
                    ProductId = soItem.ProductId,
                    ShelfId = stock.ShelfId,
                    QtyToPick = qtyToPick,
                    QtyPicked = 0,
                    UnitOfMeasureSnapshot = soItem.UnitOfMeasureSnapshot,
                    LocationSuggestion = FormatLocation(stock.Shelves),
                    IsShelfVerified = false,
                    ScannedShelfQrCode = null,
                    VerifiedAt = null
                });

                remainingQty -= qtyToPick;
            }

            if (remainingQty > 0)
            {
                throw new InvalidOperationException(
                    $"Stock product {soItem.Product?.ProductName ?? soItem.ProductId.ToString()} tidak cukup untuk dibuat Picking Task.");
            }
        } 

        salesOrder.Status = OutboundStatus.Active;

        _context.PickingTasks.Add(pickingTask);
        await _context.SaveChangesAsync();

        return await GetPickingTaskByIdAsync(pickingTask.Id);
    }

    public async Task<PickingTaskResponseDto> GetPickingTaskByIdAsync(int id)
    {
        var pickingTask = await _context.PickingTasks
            .Include(task => task.PickingItems)
                .ThenInclude(item => item.Product)
            .Include(task => task.PickingItems)
                .ThenInclude(item => item.Shelf!)
                    .ThenInclude(shelf => shelf.Zones)
            .FirstOrDefaultAsync(task => task.Id == id);

        if (pickingTask == null)
        {
            throw new KeyNotFoundException("Picking Task tidak ditemukan.");
        }

        return MapPickingTaskResponse(pickingTask);
    }

    public async Task VerifyPickingShelfAsync(VerifyPickingShelfDto dto)
    {
        var pickingItem = await _context.PickingItems
            .Include(item => item.PickingTask)
            .Include(item => item.Shelf)
            .FirstOrDefaultAsync(item => item.Id == dto.PickingItemId);

        if (pickingItem == null)
        {
            throw new KeyNotFoundException("Picking Item tidak ditemukan.");
        }

        if (pickingItem.ShelfId != dto.ShelfId)
        {
            throw new InvalidOperationException("Shelf yang discan tidak sesuai dengan lokasi picking.");
        }

        if (pickingItem.Shelf == null)
        {
            throw new KeyNotFoundException("Shelf tidak ditemukan.");
        }

        var isValidQr =
            string.Equals(dto.ScannedShelfQrCode, pickingItem.Shelf.ShelfCode, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(dto.ScannedShelfQrCode, pickingItem.Shelf.QRCodePath, StringComparison.OrdinalIgnoreCase);

        if (!isValidQr)
        {
            throw new InvalidOperationException("QR Code shelf tidak sesuai.");
        }

        var remainingQty = pickingItem.QtyToPick - pickingItem.QtyPicked;

        if (dto.QtyPicked > remainingQty)
        {
            throw new InvalidOperationException("Qty picked melebihi qty yang harus diambil.");
        }

        var stock = await _context.Stocks
            .FirstOrDefaultAsync(stock =>
                stock.ProductId == pickingItem.ProductId &&
                stock.ShelfId == pickingItem.ShelfId);

        if (stock == null)
        {
            throw new KeyNotFoundException("Stock pada shelf ini tidak ditemukan.");
        }

        if (stock.Quantity < dto.QtyPicked)
        {
            throw new InvalidOperationException("Stock pada shelf tidak mencukupi.");
        }

        stock.Quantity -= dto.QtyPicked;

        pickingItem.QtyPicked += dto.QtyPicked;
        pickingItem.ScannedShelfQrCode = dto.ScannedShelfQrCode;
        pickingItem.IsShelfVerified = pickingItem.QtyPicked >= pickingItem.QtyToPick;

        if (pickingItem.IsShelfVerified)
        {
            pickingItem.VerifiedAt = DateTime.UtcNow;
        }

        var soItem = await _context.SOItems
            .FirstOrDefaultAsync(item =>
                item.SalesOrderId == pickingItem.PickingTask!.SalesOrderId &&
                item.ProductId == pickingItem.ProductId);

        if (soItem != null)
        {
            soItem.QtyPicked += dto.QtyPicked;
        }

        pickingItem.Shelf.CurrentVolume = Math.Max(
            0,
            pickingItem.Shelf.CurrentVolume - dto.QtyPicked
        );

        await UpdatePickingTaskCompletionAsync(pickingItem.PickingTaskId);

        await _context.SaveChangesAsync();
    }

    // ==========================
    // PACKING
    // ==========================

    public async Task<PackingTaskResponseDto> CreatePackingTaskAsync(CreatePackingTaskDto dto)
    {
        var salesOrder = await _context.SalesOrders
            .Include(order => order.SOItems)
                .ThenInclude(item => item.Product)
            .FirstOrDefaultAsync(order => order.Id == dto.SalesOrderId);

        if (salesOrder == null)
        {
            throw new KeyNotFoundException("Sales Order tidak ditemukan.");
        }

        var pickingTask = await _context.PickingTasks
            .Include(task => task.PickingItems)
            .FirstOrDefaultAsync(task => task.SalesOrderId == salesOrder.Id);

        if (pickingTask == null)
        {
            throw new InvalidOperationException("Picking Task harus dibuat terlebih dahulu.");
        }

        var isPickingCompleted = pickingTask.PickingItems
            .All(item => item.QtyPicked >= item.QtyToPick && item.IsShelfVerified);

        if (!isPickingCompleted)
        {
            throw new InvalidOperationException("Picking belum selesai. Semua shelf harus diverifikasi terlebih dahulu.");
        }

        var existingPackingTask = await _context.PackingTasks
            .AnyAsync(task => task.SalesOrderId == salesOrder.Id);

        if (existingPackingTask)
        {
            throw new InvalidOperationException("Sales Order ini sudah memiliki Packing Task.");
        }

        var packingTask = new PackingTasks
        {
            PackingNumber = GenerateNumber("PACK"),
            SalesOrderId = salesOrder.Id,
            StartTime = DateTime.UtcNow,
            EndTime = null,
            TotalPackage = salesOrder.SOItems.Sum(item => item.QtyOrdered),
            PackingStatus = OutboundStatus.Active
        };

        foreach (var soItem in salesOrder.SOItems)
        {
            packingTask.PackingItems.Add(new PackingItems
            {
                ProductId = soItem.ProductId,
                QtyExpected = soItem.QtyOrdered,
                QtyVerified = 0,
                ExpectedBarcode = soItem.Product?.Barcode ?? string.Empty,
                ScannedBarcode = null,
                IsVerified = false,
                VerifiedAt = null
            });
        }

        _context.PackingTasks.Add(packingTask);
        await _context.SaveChangesAsync();

        return await GetPackingTaskByIdAsync(packingTask.Id);
    }

    public async Task<PackingTaskResponseDto> GetPackingTaskByIdAsync(int id)
    {
        var packingTask = await _context.PackingTasks
            .Include(task => task.PackingItems)
                .ThenInclude(item => item.Product)
            .FirstOrDefaultAsync(task => task.Id == id);

        if (packingTask == null)
        {
            throw new KeyNotFoundException("Packing Task tidak ditemukan.");
        }

        return MapPackingTaskResponse(packingTask);
    }

    public async Task VerifyPackingItemAsync(VerifyPackingItemDto dto)
    {
        var packingTask = await _context.PackingTasks
            .Include(task => task.PackingItems)
            .FirstOrDefaultAsync(task => task.Id == dto.PackingTaskId);

        if (packingTask == null)
        {
            throw new KeyNotFoundException("Packing Task tidak ditemukan.");
        }

        if (packingTask.PackingStatus == OutboundStatus.Completed)
        {
            throw new InvalidOperationException("Packing Task sudah selesai.");
        }

        var packingItem = packingTask.PackingItems
            .FirstOrDefault(item => item.ProductId == dto.ProductId);

        if (packingItem == null)
        {
            throw new KeyNotFoundException("Packing Item tidak ditemukan.");
        }

        if (!string.Equals(packingItem.ExpectedBarcode, dto.ScannedBarcode, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Barcode barang tidak sesuai dengan Sales Order.");
        }

        var remainingQty = packingItem.QtyExpected - packingItem.QtyVerified;

        if (dto.QtyVerified > remainingQty)
        {
            throw new InvalidOperationException("Qty verified melebihi qty yang harus diverifikasi.");
        }

        packingItem.QtyVerified += dto.QtyVerified;
        packingItem.ScannedBarcode = dto.ScannedBarcode;
        packingItem.IsVerified = packingItem.QtyVerified >= packingItem.QtyExpected;

        if (packingItem.IsVerified)
        {
            packingItem.VerifiedAt = DateTime.UtcNow;
        }

        var soItem = await _context.SOItems
            .FirstOrDefaultAsync(item =>
                item.SalesOrderId == packingTask.SalesOrderId &&
                item.ProductId == dto.ProductId);

        if (soItem != null)
        {
            soItem.QtyVerified += dto.QtyVerified;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<PackingTaskResponseDto> CompletePackingTaskAsync(CompletePackingTaskDto dto)
    {
        var packingTask = await _context.PackingTasks
            .Include(task => task.PackingItems)
            .Include(task => task.SalesOrder)
            .FirstOrDefaultAsync(task => task.Id == dto.PackingTaskId);

        if (packingTask == null)
        {
            throw new KeyNotFoundException("Packing Task tidak ditemukan.");
        }

        var isAllVerified = packingTask.PackingItems
            .All(item => item.QtyVerified >= item.QtyExpected && item.IsVerified);

        if (!isAllVerified)
        {
            throw new InvalidOperationException("Masih ada item yang belum diverifikasi barcode-nya.");
        }

        packingTask.EndTime = DateTime.UtcNow;
        packingTask.PackingStatus = OutboundStatus.Completed;

        if (packingTask.SalesOrder != null)
        {
            packingTask.SalesOrder.Status = OutboundStatus.Active;
        }

        await _context.SaveChangesAsync();

        return await GetPackingTaskByIdAsync(packingTask.Id);
    }

    // ==========================
    // SHIPMENT / SHIPPING LABEL
    // ==========================

    public async Task<ShipmentResponseDto> CreateShipmentAsync(CreateShipmentDto dto)
    {
        var packingTask = await _context.PackingTasks
            .Include(task => task.SalesOrder)
            .FirstOrDefaultAsync(task => task.Id == dto.PackingTaskId);

        if (packingTask == null)
        {
            throw new KeyNotFoundException("Packing Task tidak ditemukan.");
        }

        if (packingTask.SalesOrder == null)
        {
            throw new KeyNotFoundException("Sales Order tidak ditemukan.");
        }

        if (packingTask.PackingStatus != OutboundStatus.Completed)
        {
            throw new InvalidOperationException("Packing Task belum completed.");
        }

        var existingShipment = await _context.Shipments
            .FirstOrDefaultAsync(shipment => shipment.PackingTaskId == packingTask.Id);

        if (existingShipment != null)
        {
            throw new InvalidOperationException("Packing Task ini sudah memiliki shipment.");
        }

        var shipment = new Shipments
        {
            PackingTaskId = packingTask.Id,
            SalesOrderId = packingTask.SalesOrderId,
            ShippingLabelNumber = GenerateNumber("LBL"),
            CourierName = dto.CourierName,
            TrackingNumber = dto.TrackingNumber,
            ShippingLabelUrl = dto.ShippingLabelUrl,
            CustomerNameSnapshot = packingTask.SalesOrder.CustomerName,
            ShippingAddressSnapshot = packingTask.SalesOrder.ShippingAddress,
            ManifestDate = DateTime.UtcNow,
            Status = OutboundStatus.Completed
        };

        packingTask.SalesOrder.TrackingNumber = dto.TrackingNumber;
        packingTask.SalesOrder.Status = OutboundStatus.Completed;

        _context.Shipments.Add(shipment);
        await _context.SaveChangesAsync();

        return MapShipmentResponse(shipment);
    }

    public async Task<ShippingLabelResponseDto> GetShippingLabelAsync(int salesOrderId)
    {
        var shipment = await _context.Shipments
            .Include(shipment => shipment.SalesOrder)
            .FirstOrDefaultAsync(shipment => shipment.SalesOrderId == salesOrderId);

        if (shipment == null)
        {
            throw new KeyNotFoundException("Shipping label belum dibuat. Buat shipment terlebih dahulu.");
        }

        return new ShippingLabelResponseDto
        {
            SalesOrderId = shipment.SalesOrderId,
            SONumber = shipment.SalesOrder?.SONumber ?? string.Empty,
            CustomerName = shipment.CustomerNameSnapshot,
            ShippingAddress = shipment.ShippingAddressSnapshot,
            CourierName = shipment.CourierName,
            TrackingNumber = shipment.TrackingNumber,
            ShippingLabelNumber = shipment.ShippingLabelNumber,
            GeneratedAt = shipment.ManifestDate ?? DateTime.UtcNow
        };
    }

    // ==========================
    // PRIVATE HELPERS
    // ==========================

    private async Task UpdatePickingTaskCompletionAsync(int pickingTaskId)
    {
        var pickingTask = await _context.PickingTasks
            .Include(task => task.PickingItems)
            .FirstOrDefaultAsync(task => task.Id == pickingTaskId);

        if (pickingTask == null)
        {
            return;
        }

        var isCompleted = pickingTask.PickingItems
            .All(item => item.QtyPicked >= item.QtyToPick && item.IsShelfVerified);

        if (isCompleted)
        {
            pickingTask.EndTime = DateTime.UtcNow;
            pickingTask.PickingStatus = OutboundStatus.Completed;
        }
    }

    private static SalesOrderResponseDto MapSalesOrderResponse(SalesOrders salesOrder)
    {
        var totalOrderedQuantity = salesOrder.SOItems.Sum(item => item.QtyOrdered);
        var totalPickedItems = salesOrder.SOItems.Sum(item => item.QtyPicked);
        var totalVerifiedItems = salesOrder.SOItems.Sum(item => item.QtyVerified);

        var progressPercentage = totalOrderedQuantity == 0
            ? 0
            : Math.Round((double)totalVerifiedItems / totalOrderedQuantity * 100, 2);

        return new SalesOrderResponseDto
        {
            Id = salesOrder.Id,
            SONumber = salesOrder.SONumber,
            CustomerName = salesOrder.CustomerName,
            ShippingAddress = salesOrder.ShippingAddress,
            Courier = salesOrder.Courier,
            TrackingNumber = salesOrder.TrackingNumber,
            Status = salesOrder.Status,
            OrderDate = salesOrder.OrderDate,
            RequiredDeliveryDate = salesOrder.RequiredDeliveryDate,
            TotalItems = salesOrder.SOItems.Count,
            TotalOrderedQuantity = totalOrderedQuantity,
            TotalPickedItems = totalPickedItems,
            TotalVerifiedItems = totalVerifiedItems,
            ProgressPercentage = progressPercentage,
            IsCompleted = salesOrder.Status == OutboundStatus.Completed,
            Items = salesOrder.SOItems.Select(item => new SOItemResponseDto
            {
                Id = item.Id,
                ProductId = item.ProductId,
                SKU = item.Product?.SKU ?? string.Empty,
                ProductName = item.Product?.ProductName ?? string.Empty,
                Barcode = item.Product?.Barcode ?? string.Empty,
                UnitOfMeasure = item.UnitOfMeasureSnapshot,
                QtyOrdered = item.QtyOrdered,
                QtyPicked = item.QtyPicked,
                QtyVerified = item.QtyVerified,
                FormattedQuantity = $"Qty: {item.QtyOrdered} {item.UnitOfMeasureSnapshot}"
            }).ToList()
        };
    }

    private static PickingTaskResponseDto MapPickingTaskResponse(PickingTasks pickingTask)
    {
        var totalQty = pickingTask.PickingItems.Sum(item => item.QtyToPick);
        var pickedQty = pickingTask.PickingItems.Sum(item => item.QtyPicked);

        var progressPercentage = totalQty == 0
            ? 0
            : Math.Round((double)pickedQty / totalQty * 100, 2);

        return new PickingTaskResponseDto
        {
            Id = pickingTask.Id,
            SalesOrderId = pickingTask.SalesOrderId,
            PickingNumber = pickingTask.PickingNumber,
            StartTime = pickingTask.StartTime,
            EndTime = pickingTask.EndTime,
            TotalItems = totalQty,
            PickedItems = pickedQty,
            ProgressPercentage = progressPercentage,
            PickingStatus = pickingTask.PickingStatus,
            IsCompleted = pickingTask.PickingStatus == OutboundStatus.Completed,
            Items = pickingTask.PickingItems.Select(item => new PickingItemResponseDto
            {
                Id = item.Id,
                ProductId = item.ProductId,
                SKU = item.Product?.SKU ?? string.Empty,
                ProductName = item.Product?.ProductName ?? string.Empty,
                ShelfId = item.ShelfId,
                ShelfCode = item.Shelf?.ShelfCode ?? string.Empty,
                ZoneCode = item.Shelf?.Zones?.ZoneCode ?? string.Empty,
                ZoneName = item.Shelf?.Zones?.ZoneName ?? string.Empty,
                Aisle = item.Shelf?.Aisle ?? 0,
                LocationSuggestion = item.LocationSuggestion,
                QtyToPick = item.QtyToPick,
                QtyPicked = item.QtyPicked,
                UnitOfMeasure = item.UnitOfMeasureSnapshot,
                FormattedQuantity = $"Ambil: {item.QtyToPick} {item.UnitOfMeasureSnapshot}",
                IsShelfVerified = item.IsShelfVerified,
                ScannedShelfQrCode = item.ScannedShelfQrCode
            }).ToList()
        };
    }

    private static PackingTaskResponseDto MapPackingTaskResponse(PackingTasks packingTask)
    {
        var totalExpected = packingTask.PackingItems.Sum(item => item.QtyExpected);
        var verifiedItems = packingTask.PackingItems.Sum(item => item.QtyVerified);

        var progressPercentage = totalExpected == 0
            ? 0
            : Math.Round((double)verifiedItems / totalExpected * 100, 2);

        return new PackingTaskResponseDto
        {
            Id = packingTask.Id,
            SalesOrderId = packingTask.SalesOrderId,
            PackingNumber = packingTask.PackingNumber,
            StartTime = packingTask.StartTime,
            EndTime = packingTask.EndTime,
            TotalPackage = totalExpected,
            VerifiedItems = verifiedItems,
            ProgressPercentage = progressPercentage,
            PackingStatus = packingTask.PackingStatus,
            IsCompleted = packingTask.PackingStatus == OutboundStatus.Completed,
            Items = packingTask.PackingItems.Select(item => new PackingItemResponseDto
            {
                Id = item.Id,
                ProductId = item.ProductId,
                SKU = item.Product?.SKU ?? string.Empty,
                ProductName = item.Product?.ProductName ?? string.Empty,
                ExpectedBarcode = item.ExpectedBarcode,
                ScannedBarcode = item.ScannedBarcode,
                QtyExpected = item.QtyExpected,
                QtyVerified = item.QtyVerified,
                UnitOfMeasure = item.Product?.UnitOfMeasure ?? string.Empty,
                FormattedQuantity = $"Qty: {item.QtyExpected} {item.Product?.UnitOfMeasure ?? string.Empty}",
                IsVerified = item.IsVerified
            }).ToList()
        };
    }

    private static ShipmentResponseDto MapShipmentResponse(Shipments shipment)
    {
        return new ShipmentResponseDto
        {
            Id = shipment.Id,
            PackingTaskId = shipment.PackingTaskId,
            SalesOrderId = shipment.SalesOrderId,
            ShippingLabelNumber = shipment.ShippingLabelNumber,
            CustomerName = shipment.CustomerNameSnapshot,
            ShippingAddress = shipment.ShippingAddressSnapshot,
            CourierName = shipment.CourierName,
            TrackingNumber = shipment.TrackingNumber,
            ShippingLabelUrl = shipment.ShippingLabelUrl,
            ManifestDate = shipment.ManifestDate,
            Status = shipment.Status
        };
    }

    private static string GenerateNumber(string prefix)
    {
        return $"{prefix}-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
    }

    private static string FormatLocation(Shelves? shelf)
    {
        if (shelf == null)
        {
            return "Lokasi shelf tidak ditemukan.";
        }

        var zoneCode = shelf.Zones?.ZoneCode ?? "-";
        var zoneName = shelf.Zones?.ZoneName ?? "-";

        return $"Zone {zoneCode} ({zoneName}) - Aisle {shelf.Aisle} - Shelf {shelf.ShelfCode}";
    }

    private static void ValidateOutboundStatus(string status)
    {
        var validStatuses = new[]
        {
            OutboundStatus.Queued,
            OutboundStatus.Active,
            OutboundStatus.Completed
        };

        if (!validStatuses.Contains(status))
        {
            throw new InvalidOperationException(
                "Status outbound tidak valid. Gunakan Queued, Active, atau Completed.");
        }
    }
}