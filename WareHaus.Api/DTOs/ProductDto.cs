using System.ComponentModel.DataAnnotations;

namespace WareHaus.Api.DTOs;

public record GetProductDto(
    int Id,
    string SKU,
    string ProductName,
    string Barcode,
    string UnitOfMeasure,
    int CurrentStock
);

public record GetProductDetailDto(
    int Id,
    string SKU,
    string ProductName,
    string Barcode,
    string UnitOfMeasure,
    int CurrentStock,
    List<GetProductStockLocationDto> Stocks
);

public record GetProductStockLocationDto(
    int ShelfId,
    string ShelfCode,
    int ZoneId,
    string ZoneCode,
    string ZoneName,
    int Aisle,
    string LocationName,
    int Quantity,
    int ShelfCapacity,
    int ShelfCurrentVolume,
    int ShelfAvailableCapacity,
    string QRCodePath
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

public record AddProductStockLocationDto
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Product id harus lebih dari 0.")]
    public int ProductId { get; init; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Shelf id harus lebih dari 0.")]
    public int ShelfId { get; init; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity harus lebih dari 0.")]
    public int Quantity { get; init; }
}
public record MoveProductStockLocationDto
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Product id harus lebih dari 0.")]
    public int ProductId { get; init; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Source shelf id harus lebih dari 0.")]
    public int FromShelfId { get; init; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Destination shelf id harus lebih dari 0.")]
    public int ToShelfId { get; init; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity harus lebih dari 0.")]
    public int Quantity { get; init; }
}
public record UpdateProductStockLocationDto
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Product id harus lebih dari 0.")]
    public int ProductId { get; init; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Shelf id harus lebih dari 0.")]
    public int ShelfId { get; init; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity harus lebih dari 0.")]
    public int Quantity { get; init; }
}