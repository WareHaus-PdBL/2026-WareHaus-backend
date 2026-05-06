namespace WareHaus.Api.DTOs;

public record CreatePurchaseOrderDto(
    string PONumber,
    string SupplierName,
    List<CreatePOItemDto> Items
);

public record CreatePOItemDto(
    int ProductId,
    int QtyExpected
);

public record ReceiveItemDto(
    int POItemId,
    int QtyReceived,
    string Condition,
    DateTime? ExpiryDate
);

public record PutawayDto(
    string ShelfCode,
    int ProductId,
    int Quantity
);