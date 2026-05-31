using System.ComponentModel.DataAnnotations;

namespace WareHaus.Api.DTOs;

// SALES ORDER DTO

public record CreateSalesOrderDto
{
    [Required(ErrorMessage = "Customer name wajib diisi.")]
    public string CustomerName { get; init; } = string.Empty;

    [Required(ErrorMessage = "Shipping address wajib diisi.")]
    public string ShippingAddress { get; init; } = string.Empty;

    [Required(ErrorMessage = "Courier wajib diisi.")]
    public string Courier { get; init; } = string.Empty;

    [Required(ErrorMessage = "Required delivery date wajib diisi.")]
    public DateTime RequiredDeliveryDate { get; init; }

    [Required(ErrorMessage = "Item sales order wajib diisi.")]
    [MinLength(1, ErrorMessage = "Minimal harus ada 1 item dalam sales order.")]
    public List<CreateSOItemDto> Items { get; init; } = new();
}

public record UpdateSalesOrderDto
{
    public string? CustomerName { get; init; }

    public string? ShippingAddress { get; init; }

    public string? Courier { get; init; }

    public DateTime? RequiredDeliveryDate { get; init; }

    public string? TrackingNumber { get; init; }

    public string? Status { get; init; }
}

public record CreateSOItemDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Product id harus lebih dari 0.")]
    public int ProductId { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "Qty ordered harus lebih dari 0.")]
    public int QtyOrdered { get; init; }
}

public record SalesOrderResponseDto
{
    public int Id { get; init; }

    public string SONumber { get; init; } = string.Empty;

    public string CustomerName { get; init; } = string.Empty;

    public string ShippingAddress { get; init; } = string.Empty;

    public string Courier { get; init; } = string.Empty;

    public string? TrackingNumber { get; init; }

    public string Status { get; init; } = string.Empty;

    public DateTime OrderDate { get; init; }

    public DateTime RequiredDeliveryDate { get; init; }

    public int TotalItems { get; init; }

    public int TotalOrderedQuantity { get; init; }

    public int TotalPickedItems { get; init; }

    public int TotalVerifiedItems { get; init; }

    public double ProgressPercentage { get; init; }

    public bool IsCompleted { get; init; }

    public List<SOItemResponseDto> Items { get; init; } = new();
}

public record SOItemResponseDto
{
    public int Id { get; init; }

    public int ProductId { get; init; }

    public string SKU { get; init; } = string.Empty;

    public string ProductName { get; init; } = string.Empty;

    public string Barcode { get; init; } = string.Empty;

    public string UnitOfMeasure { get; init; } = string.Empty;

    public int QtyOrdered { get; init; }

    public int QtyPicked { get; init; }

    public int QtyVerified { get; init; }

    public string FormattedQuantity { get; init; } = string.Empty;
}

// PICKING DTO

public record CreatePickingTaskDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Sales order id harus lebih dari 0.")]
    public int SalesOrderId { get; init; }
}

public record VerifyPickingShelfDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Picking item id harus lebih dari 0.")]
    public int PickingItemId { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "Shelf id harus lebih dari 0.")]
    public int ShelfId { get; init; }

    [Required(ErrorMessage = "Shelf QR code wajib diisi.")]
    public string ScannedShelfQrCode { get; init; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Qty picked harus lebih dari 0.")]
    public int QtyPicked { get; init; }
}

public record PickingTaskResponseDto
{
    public int Id { get; init; }

    public int SalesOrderId { get; init; }

    public string PickingNumber { get; init; } = string.Empty;

    public DateTime? StartTime { get; init; }

    public DateTime? EndTime { get; init; }

    public int TotalItems { get; init; }

    public int PickedItems { get; init; }

    public double ProgressPercentage { get; init; }

    public string PickingStatus { get; init; } = string.Empty;

    public bool IsCompleted { get; init; }

    public List<PickingItemResponseDto> Items { get; init; } = new();
}

public record PickingItemResponseDto
{
    public int Id { get; init; }

    public int ProductId { get; init; }

    public string SKU { get; init; } = string.Empty;

    public string ProductName { get; init; } = string.Empty;

    public int ShelfId { get; init; }

    public string ShelfCode { get; init; } = string.Empty;

    public string ZoneCode { get; init; } = string.Empty;

    public string ZoneName { get; init; } = string.Empty;

    public int Aisle { get; init; }

    public string LocationSuggestion { get; init; } = string.Empty;

    public int QtyToPick { get; init; }

    public int QtyPicked { get; init; }

    public string UnitOfMeasure { get; init; } = string.Empty;

    public string FormattedQuantity { get; init; } = string.Empty;

    public bool IsShelfVerified { get; init; }

    public string? ScannedShelfQrCode { get; init; }
}

// PACKING DTO

public record CreatePackingTaskDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Sales order id harus lebih dari 0.")]
    public int SalesOrderId { get; init; }
}

public record VerifyPackingItemDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Packing task id harus lebih dari 0.")]
    public int PackingTaskId { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "Product id harus lebih dari 0.")]
    public int ProductId { get; init; }

    [Required(ErrorMessage = "Scanned barcode wajib diisi.")]
    public string ScannedBarcode { get; init; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Qty verified harus lebih dari 0.")]
    public int QtyVerified { get; init; }
}

public record CompletePackingTaskDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Packing task id harus lebih dari 0.")]
    public int PackingTaskId { get; init; }
}

public record PackingTaskResponseDto
{
    public int Id { get; init; }

    public int SalesOrderId { get; init; }

    public string PackingNumber { get; init; } = string.Empty;

    public DateTime? StartTime { get; init; }

    public DateTime? EndTime { get; init; }

    public int TotalPackage { get; init; }

    public int VerifiedItems { get; init; }

    public double ProgressPercentage { get; init; }

    public string PackingStatus { get; init; } = string.Empty;

    public bool IsCompleted { get; init; }

    public List<PackingItemResponseDto> Items { get; init; } = new();
}

public record PackingItemResponseDto
{
    public int Id { get; init; }

    public int ProductId { get; init; }

    public string SKU { get; init; } = string.Empty;

    public string ProductName { get; init; } = string.Empty;

    public string ExpectedBarcode { get; init; } = string.Empty;

    public string? ScannedBarcode { get; init; }

    public int QtyExpected { get; init; }

    public int QtyVerified { get; init; }

    public string UnitOfMeasure { get; init; } = string.Empty;

    public string FormattedQuantity { get; init; } = string.Empty;

    public bool IsVerified { get; init; }
}

// ==========================
// SHIPMENT / SHIPPING LABEL DTO
// ==========================

public record CreateShipmentDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Packing task id harus lebih dari 0.")]
    public int PackingTaskId { get; init; }

    [Required(ErrorMessage = "Courier name wajib diisi.")]
    public string CourierName { get; init; } = string.Empty;

    [Required(ErrorMessage = "Tracking number wajib diisi.")]
    public string TrackingNumber { get; init; } = string.Empty;

    public string? ShippingLabelUrl { get; init; }
}

public record ShipmentResponseDto
{
    public int Id { get; init; }

    public int PackingTaskId { get; init; }

    public int SalesOrderId { get; init; }

    public string ShippingLabelNumber { get; init; } = string.Empty;

    public string CustomerName { get; init; } = string.Empty;

    public string ShippingAddress { get; init; } = string.Empty;

    public string CourierName { get; init; } = string.Empty;

    public string TrackingNumber { get; init; } = string.Empty;

    public string? ShippingLabelUrl { get; init; }

    public DateTime? ManifestDate { get; init; }

    public string Status { get; init; } = string.Empty;
}

public record ShippingLabelResponseDto
{
    public int SalesOrderId { get; init; }

    public string SONumber { get; init; } = string.Empty;

    public string CustomerName { get; init; } = string.Empty;

    public string ShippingAddress { get; init; } = string.Empty;

    public string CourierName { get; init; } = string.Empty;

    public string TrackingNumber { get; init; } = string.Empty;

    public string ShippingLabelNumber { get; init; } = string.Empty;

    public DateTime GeneratedAt { get; init; }
}