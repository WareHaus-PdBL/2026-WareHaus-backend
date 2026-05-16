using WareHaus.Api.DTOs;

namespace WareHaus.Api.Services;

public interface IZoneService
{
    Task<List<GetAllZonesDto>> GetAllZonesAsync();
    Task<GetDetailsZoneDto> GetDetailsZoneAsync(int zoneId, int? aisle);
    Task<GetAllZonesDto> CreateZoneAsync(CreateZoneDto createZoneDto);
    Task<GetAllZonesDto> UpdateZoneAsync(int zoneId, UpdateZoneDto updateZoneDto);
    Task<DownloadQrFileDto> DownloadQRCodeShelfAsync(int shelfId, string option);
    Task<DownloadQrFileDto> DownloadQRCodeAisleAsync(int zoneId, int aisle, string option);
    Task<byte[]> DownloadAisleQrCodesPdfAsync(int zoneId, int aisle);
    Task DeleteZoneAsync(int zoneId);
    Task<GetShelfForZoneDetailsDto> GetShelfDetailsAsync(int shelfId);
    Task<List<GetShelfForZoneDto>> GetShelvesInZoneAsync(int zoneId);
}