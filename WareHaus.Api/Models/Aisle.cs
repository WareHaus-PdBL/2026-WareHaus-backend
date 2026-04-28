namespace WareHaus.API.Models;

public class Aisle : BaseEntity
{
    public int ZoneId { get; set; }

    public string AisleCode { get; set; } = string.Empty;

    public string AisleName { get; set; } = string.Empty;

    public Zone Zone { get; set; } = null!;

    public ICollection<Shelf> Shelves { get; set; } = new List<Shelf>();
}