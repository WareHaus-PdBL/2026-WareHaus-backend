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

public record CreateZoneDto(
    [Required]
    string ZoneCode,
    [Required]
    string ZoneName,
    string Description,
    string Category,
    [Required, Range(1, int.MaxValue, ErrorMessage = "Lorong harus lebih dari 0.")]
    int TotalAisle,
    [Required, Range(1, int.MaxValue, ErrorMessage = "Rak per lorong harus lebih dari 0.")]
    int ShelfPerAisle,
    [Required, Range(1, int.MaxValue, ErrorMessage = "Kapasitas per rak harus lebih dari 0.")]
    int CapacityPerShelf
);

public record UpdateZoneDto(
    string? ZoneName,
    string? Category,
    string? Description
);

