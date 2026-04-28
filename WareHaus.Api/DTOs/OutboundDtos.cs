namespace WareHaus.Api.DTOs;

public record CreateSalesOrderDto(
    string SONumber,
    string CustomerName,
    List<CreateSOItemDto> Items
);

public record CreateSOItemDto(
    Guid ProductId,
    int Quantity
);

public record PackingDto(
    Guid SalesOrderId,
    Guid ProductId,
    int QuantityPacked,
    string PackedBy
);

public record ShippingDto(
    Guid SalesOrderId,
    string CourierName,
    string TrackingNumber
);