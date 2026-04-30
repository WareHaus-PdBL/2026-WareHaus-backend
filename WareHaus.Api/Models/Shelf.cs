namespace WareHaus.Api.Models;

public class Shelf : BaseEntity
{
    public int ZoneId { get; set; }

    public string ShelfCode { get; set; } = string.Empty;

    public int Aisle { get; set; }

    public int Capacity { get; set; }

    public int CurrentVolume { get; set; }

    public string QRCodePath { get; set; } = string.Empty;

    public Zone? Zone { get; set; }

    public ICollection<Stock> Stocks { get; set; } = new List<Stock>();
}