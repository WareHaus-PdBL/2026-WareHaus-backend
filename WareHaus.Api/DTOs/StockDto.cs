namespace WareHaus.Api.DTOs;

public record GetStockDto(
    int Id,
    int ShelfId,
    int ProductId,
    int Quantity,
    GetShelfForZoneDto Shelf
);

public record GetStocksInShelfDto(
    int Id,
    int ShelfId,
    int ProductId,
    int Quantity,
    GetProductDto Product
);