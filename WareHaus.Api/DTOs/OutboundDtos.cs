namespace WareHaus.Api.DTOs;

public record CreateSalesOrderDto(
    string? CustomerName,
    List<CreateSOItemDto> Items
);

public record UpdateSalesOrderDto(
    string? CustomerName,
    string? Status
);

public record CreateSOItemDto(
    int ProductId,
    int QtyOrdered
);

public record SalesOrderResponseDto(
    int Id,
    string SONumber,
    string CustomerName,
    string Status,
    DateTime OrderDate,
    int TotalItems,
    int TotalPickedItems,
    double ProgressPercentage,
    bool IsCompleted,
    List<SOItemResponseDto> Items
);

public record SOItemResponseDto(
    int Id,
    int ProductId,
    int QtyOrdered,
    int QtyPicked
);

public record CreatePackingTaskDto(
    int SalesOrderId
);

public record VerifyPackingItemDto(
    int PackingTaskId,
    int ProductId,
    int QtyVerified
);

public record CompletePackingTaskDto(
    int PackingTaskId
);

public record PackingTaskResponseDto(
    int Id,
    int SalesOrderId,
    DateTime? StartTime,
    DateTime? EndTime,
    int TotalPackage,
    int VerifiedItems,
    double ProgressPercentage,
    string PackingStatus,
    bool IsCompleted
);

public record CreateShipmentDto(
    int PackingTaskId,
    string CourierName,
    string TrackingNumber,
    string ShippingLabelUrl
);

public record ShipmentResponseDto(
    int Id,
    int PackingTaskId,
    string CourierName,
    string TrackingNumber,
    string ShippingLabelUrl,
    DateTime? ManifestDate,
    string Status
);