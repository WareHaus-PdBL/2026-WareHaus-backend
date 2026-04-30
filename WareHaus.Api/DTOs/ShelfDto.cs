namespace WareHaus.Api.DTOs;

public record GetShelfForZoneDto(
    int Id,
    string ShelfCode,
    int Aisle,
    int Capacity,
    int CurrentVolume,
    string QRCodePath
);