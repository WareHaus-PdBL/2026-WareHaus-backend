namespace WareHaus.Api.Models;

public class Shelves : BaseEntities
{
    public int ZoneId { get; set; }

    public string Aisle { get; set; } = string.Empty;

    public string ShelfCode { get; set; } = string.Empty;

    public int Capacity { get; set; }

    public int CurrentVolume { get; set; }

    public string? QRCodePath { get; set; }

    public Zones? Zones { get; set; }

    public ICollection<Stocks> Stocks { get; set; } = new List<Stocks>();
}