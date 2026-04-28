namespace WareHaus.Api.Models;

public class Shelves : BaseEntities
{
    public Guid ZoneId { get; set; }

    public string ShelfCode { get; set; } = string.Empty;

    public int Capacity { get; set; }

    public int CurrentVolume { get; set; }

    public string? QRCodePath { get; set; }

    public Zones Zone { get; set; } = null!;

    public ICollection<Stocks> Stocks { get; set; } = new List<Stocks>();
}