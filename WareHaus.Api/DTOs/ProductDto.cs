namespace WareHaus.Api.DTOs;

public record GetProductDto(
    int Id,
    string SKU,
    string ProductName,
    string Barcode,
    string UnitOfMeasure
);

public record GetProductDetailDto(
    int Id,
    string SKU,
    string ProductName,
    string Barcode,
    string UnitOfMeasure,
    ICollection<GetStockDto> Stocks
);

public record CreateProductDto(
    string SKU,
    string ProductName,
    string Barcode,
    string UnitOfMeasure
);

public record UpdateProductDto(
    string? SKU,
    string? ProductName,
    string? Barcode,
    string? UnitOfMeasure
);