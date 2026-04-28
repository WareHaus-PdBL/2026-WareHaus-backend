namespace WareHaus.Api.DTOs.Zone;

public class GetAllZonesDto
{
    public int Id { get; set; }
    public string ZoneCode { get; set; } = string.Empty;
    public string ZoneName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int TotalAisle { get; set; }
    public int ShelfPerAisle { get; set; }
}