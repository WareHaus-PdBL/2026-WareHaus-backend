using WareHaus.Api.DTOs.Zone;

namespace WareHaus.Api.Services;

public interface IZoneService
{
    Task<List<GetAllZonesDto>> GetAllZonesAsync();
    Task<GetDetailsZoneDto> GetDetailsZoneAsync(int zoneId, int? aisle);
    Task<GetAllZonesDto> CreateZoneAsync(CreateZoneDto createZoneDto);
    Task<GetAllZonesDto> UpdateZoneAsync(int zoneId, UpdateZoneDto updateZoneDto);
    Task DeleteZoneAsync(int zoneId);
}