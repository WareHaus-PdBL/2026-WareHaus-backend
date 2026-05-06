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

    public async Task<List<SalesOrderResponseDto>> GetSalesOrdersAsync()
    {
        var salesOrders = await _context.SalesOrders
            .Include(order => order.SOItems)
            .Where(order => order.DeletedAt == null)
            .OrderBy(order => order.Id)
            .ToListAsync();

        return salesOrders.Select(MapSalesOrderResponse).ToList();
    }

    public async Task<SalesOrderResponseDto> GetSalesOrderByIdAsync(int id)
    {
        var salesOrder = await _context.SalesOrders
            .Include(order => order.SOItems)
            .FirstOrDefaultAsync(order => order.Id == id && order.DeletedAt == null);

        if (salesOrder == null)
        {
            throw new KeyNotFoundException("Sales Order tidak ditemukan");
        }

        return MapSalesOrderResponse(salesOrder);
    }

    public async Task<SalesOrderResponseDto> CreateSalesOrderAsync(CreateSalesOrderDto dto)
    {
        if (dto.Items == null || !dto.Items.Any())
        {
            throw new InvalidOperationException("Sales Order harus memiliki minimal satu item");
        }

        var salesOrder = new SalesOrders
        {
            SONumber = GenerateSONumber(),
            CustomerName = string.IsNullOrWhiteSpace(dto.CustomerName) ? "Someone" : dto.CustomerName,
            Status = "Pending",
            OrderDate = DateTime.UtcNow
        };

        foreach (var item in dto.Items)
        {
            if (item.QtyOrdered <= 0)
            {
                throw new InvalidOperationException("Qty ordered harus lebih dari 0");
            }

            var productExists = await _context.Products
                .AnyAsync(product => product.Id == item.ProductId && product.DeletedAt == null);

            if (!productExists)
            {
                throw new KeyNotFoundException($"Product dengan ID {item.ProductId} tidak ditemukan");
            }

            salesOrder.SOItems.Add(new SOItems
            {
                ProductId = item.ProductId,
                QtyOrdered = item.QtyOrdered,
                QtyPicked = 0
            });
        }

        _context.SalesOrders.Add(salesOrder);
        await _context.SaveChangesAsync();

        return MapSalesOrderResponse(salesOrder);
    }

    public async Task<SalesOrderResponseDto> UpdateSalesOrderAsync(int id, UpdateSalesOrderDto dto)
    {
        var salesOrder = await _context.SalesOrders
            .Include(order => order.SOItems)
            .FirstOrDefaultAsync(order => order.Id == id && order.DeletedAt == null);

        if (salesOrder == null)
        {
            throw new KeyNotFoundException("Sales Order tidak ditemukan");
        }

        if (!string.IsNullOrWhiteSpace(dto.CustomerName))
        {
            salesOrder.CustomerName = dto.CustomerName;
        }

        if (!string.IsNullOrWhiteSpace(dto.Status))
        {
            salesOrder.Status = dto.Status;
        }

        await _context.SaveChangesAsync();

        return MapSalesOrderResponse(salesOrder);
    }

    public async Task DeleteSalesOrderAsync(int id)
    {
        var salesOrder = await _context.SalesOrders
            .FirstOrDefaultAsync(order => order.Id == id && order.DeletedAt == null);

        if (salesOrder == null)
        {
            throw new KeyNotFoundException("Sales Order tidak ditemukan");
        }

        _context.SalesOrders.Remove(salesOrder);
        await _context.SaveChangesAsync();
    }

    public async Task<PackingTaskResponseDto> CreatePackingTaskAsync(CreatePackingTaskDto dto)
    {
        var salesOrder = await _context.SalesOrders
            .Include(order => order.SOItems)
            .FirstOrDefaultAsync(order =>
                order.Id == dto.SalesOrderId &&
                order.DeletedAt == null);

        if (salesOrder == null)
        {
            throw new KeyNotFoundException("Sales Order tidak ditemukan");
        }

        if (!salesOrder.SOItems.Any())
        {
            throw new InvalidOperationException("Sales Order belum memiliki item");
        }

        var existingPackingTask = await _context.PackingTasks
            .FirstOrDefaultAsync(task =>
                task.SOId == salesOrder.Id &&
                task.DeletedAt == null &&
                task.PackingStatus != "Cancelled");

        if (existingPackingTask != null)
        {
            throw new InvalidOperationException("Sales Order ini sudah memiliki Packing Task");
        }

        var packingTask = new PackingTasks
        {
            SOId = salesOrder.Id,
            StartTime = DateTime.UtcNow,
            EndTime = null,
            TotalPackage = salesOrder.SOItems.Sum(item => item.QtyOrdered),
            PackingStatus = "In Progress"
        };

        salesOrder.Status = "Packing";

        _context.PackingTasks.Add(packingTask);
        await _context.SaveChangesAsync();

        return MapPackingTaskResponse(packingTask);

    }


    public async Task VerifyPackingItemAsync(VerifyPackingItemDto dto)
    {
        var packingTask = await _context.PackingTasks
            .FirstOrDefaultAsync(task =>
                task.Id == dto.PackingTaskId &&
                task.DeletedAt == null);

        if (packingTask == null)
        {
            throw new KeyNotFoundException("Packing Task tidak ditemukan");
        }

        if (packingTask.PackingStatus == "Completed")
        {
            throw new InvalidOperationException("Packing Task sudah selesai");
        }

        var salesOrderItem = await _context.SOItems
            .FirstOrDefaultAsync(item =>
                item.SOId == packingTask.SOId &&
                item.ProductId == dto.ProductId &&
                item.DeletedAt == null);

        if (salesOrderItem == null)
        {
            throw new KeyNotFoundException("Item Sales Order tidak ditemukan");
        }

        if (dto.QtyVerified <= 0)
        {
            throw new InvalidOperationException("Qty verified harus lebih dari 0");
        }

        var remainingQty = salesOrderItem.QtyOrdered - salesOrderItem.QtyPicked;

        if (dto.QtyVerified > remainingQty)
        {
            throw new InvalidOperationException("Qty verified melebihi qty yang belum dipicking");
        }

        salesOrderItem.QtyPicked += dto.QtyVerified;

        var packingItem = new PackingItems
        {
            PackingTaskId = packingTask.Id,
            ProductId = dto.ProductId,
            QtyVerified = dto.QtyVerified
        };

        _context.PackingItems.Add(packingItem);
        await _context.SaveChangesAsync();
    }

    public async Task<PackingTaskResponseDto> CompletePackingTaskAsync(CompletePackingTaskDto dto)
    {
        var packingTask = await _context.PackingTasks
            .Include(task => task.SalesOrders)
            .FirstOrDefaultAsync(task =>
                task.Id == dto.PackingTaskId &&
                task.DeletedAt == null);

        if (packingTask == null)
        {
            throw new KeyNotFoundException("Packing Task tidak ditemukan");
        }

        var salesOrderItems = await _context.SOItems
            .Where(item =>
                item.SOId == packingTask.SOId &&
                item.DeletedAt == null)
            .ToListAsync();

        var isAllPicked = salesOrderItems.All(item => item.QtyPicked >= item.QtyOrdered);

        if (!isAllPicked)
        {
            throw new InvalidOperationException("Masih ada item yang belum selesai dipicking");
        }

        packingTask.EndTime = DateTime.UtcNow;
        packingTask.PackingStatus = "Completed";

        if (packingTask.SalesOrders != null)
        {
            packingTask.SalesOrders.Status = "Packing";
        }

        await _context.SaveChangesAsync();

        return MapPackingTaskResponse(packingTask);
    }

    public async Task<ShipmentResponseDto> CreateShipmentAsync(CreateShipmentDto dto)
    {
        var packingTask = await _context.PackingTasks
            .Include(task => task.SalesOrders)
            .FirstOrDefaultAsync(task =>
                task.Id == dto.PackingTaskId &&
                task.DeletedAt == null);

        if (packingTask == null)
        {
            throw new KeyNotFoundException("Packing Task tidak ditemukan");
        }

        if (packingTask.PackingStatus != "Completed")
        {
            throw new InvalidOperationException("Packing Task belum completed");
        }

        var shipment = new Shipments
        {
            PackingTaskId = packingTask.Id,
            CourierName = string.IsNullOrWhiteSpace(dto.CourierName) ? "NONE" : dto.CourierName,
            TrackingNumber = dto.TrackingNumber,
            ShippingLabelUrl = dto.ShippingLabelUrl,
            ManifestDate = DateTime.UtcNow,
            Status = "Ready"
        };

        if (packingTask.SalesOrders != null)
        {
            packingTask.SalesOrders.Status = "Shipped";
        }

        _context.Shipments.Add(shipment);
        await _context.SaveChangesAsync();

        return new ShipmentResponseDto(
            shipment.Id,
            shipment.PackingTaskId,
            shipment.CourierName,
            shipment.TrackingNumber,
            shipment.ShippingLabelUrl,
            shipment.ManifestDate,
            shipment.Status
        );
    }

    private static string GenerateSONumber()
    {
        return $"SO-{DateTime.UtcNow:yyyyMMddHHmmss}";
    }

    private static SalesOrderResponseDto MapSalesOrderResponse(SalesOrders salesOrder)
    {
        return new SalesOrderResponseDto(
            salesOrder.Id,
            salesOrder.SONumber,
            salesOrder.CustomerName,
            salesOrder.Status,
            salesOrder.OrderDate,
            salesOrder.SOItems
                .Select(item => new SOItemResponseDto(
                    item.Id,
                    item.ProductId,
                    item.QtyOrdered,
                    item.QtyPicked))
                .ToList()
        );
    }

    private static PackingTaskResponseDto MapPackingTaskResponse(PackingTasks packingTask)
    {
        return new PackingTaskResponseDto(
            packingTask.Id,
            packingTask.SOId,
            packingTask.StartTime,
            packingTask.EndTime,
            packingTask.TotalPackage,
            packingTask.PackingStatus
        );
    }
}