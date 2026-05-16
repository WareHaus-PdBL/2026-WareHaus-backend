namespace WareHaus.Api.DTOs;

public record GetShelfForZoneDto(
    int Id,
    string ShelfCode,
    int Aisle,
    int Capacity,
    int CurrentVolume,
    string QRCodePath
);

public record GetShelfForZoneDetailsDto(
    int Id,
    string ShelfCode,
    int Aisle,
    int Capacity,
    int CurrentVolume,
    string QRCodePath,
    List<GetStocksInShelfDto> Stocks
);