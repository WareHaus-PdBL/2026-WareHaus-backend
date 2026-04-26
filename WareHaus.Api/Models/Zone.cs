namespace WareHaus.API.Models;

public class Zone : BaseEntity
{
    public string ZoneCode { get; set; } = string.Empty;

    public string ZoneName { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public int TotalAisle { get; set; }

    public int ShelfPerAisle { get; set; }

    public int LevelPerShelf { get; set; }

    public ICollection<Shelf> Shelves { get; set; } = new List<Shelf>();
}