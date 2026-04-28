namespace WareHaus.API.Models;

public class Shelf : BaseEntity
{
    public int AisleId { get; set; }

    public string ShelfCode { get; set; } = string.Empty;

    public int Capacity { get; set; }

    public int CurrentVolume { get; set; }

    public string? QRCodePath { get; set; }

    public Aisle Aisle { get; set; } = null!;

    public ICollection<Stock> Stocks { get; set; } = new List<Stock>();
}