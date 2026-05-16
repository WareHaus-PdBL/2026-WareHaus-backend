using System.ComponentModel.DataAnnotations;

namespace WareHaus.Api.DTOs;

public record GetAllZonesDto(
    int Id,
    string ZoneCode,
    string ZoneName,
    string Category,
    string Description,
    int TotalAisle,
    int ShelfPerAisle
);

public record GetDetailsZoneDto(
    int Id,
    string ZoneCode,
    string ZoneName,
    string Category,
    string Description,
    int TotalAisle,
    int ShelfPerAisle,
    int EmptyShelves,
    List<GetAisleDto>? Aisle,
    List<GetShelfForZoneDto>? Shelves
);

public record CreateZoneDto
{
    [Required(ErrorMessage = "Zone code wajib diisi.")]
    public string ZoneCode { get; init; } = string.Empty;

    [Required(ErrorMessage = "Zone name wajib diisi.")]
    public string ZoneName { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    [Required(ErrorMessage = "Category wajib diisi.")]
    public string Category { get; init; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Total aisle harus lebih dari 0.")]
    public int TotalAisle { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "Shelf per aisle harus lebih dari 0.")]
    public int ShelfPerAisle { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "Capacity per shelf harus lebih dari 0.")]
    public int CapacityPerShelf { get; init; }
}

public record UpdateZoneDto(
    string? ZoneName,
    string? Category,
    string? Description
);