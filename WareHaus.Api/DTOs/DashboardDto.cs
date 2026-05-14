namespace WareHaus.Api.DTOs;

public record RecentStockLogDto(
    int Id,
    string Type,
    string Title,
    string ProductName,
    string SKU,
    string LocationName,
    int Quantity,
    int StockAfterMovement,
    string Time,
    DateTime CreatedAt
);