using WareHaus.Api.DTOs.Shelf;

namespace WareHaus.Api.DTOs.Zone;

public class GetDetailsZoneDto : GetAllZonesDto
{
    public List<GetShelfForZoneDto> Shelves { get; set; } = new List<GetShelfForZoneDto>();
}