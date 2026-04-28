namespace WareHaus.Api.DTOs.Shelf;

public class GetShelfForZoneDto
{
    public int Id { get; set; }
    public string ShelfCode { get; set; } = string.Empty;
    public int Aisle { get; set; }
    public int Capacity { get; set; }
    public int CurrentVolume { get; set; }
    public string QRCodePath { get; set; } = string.Empty;
}