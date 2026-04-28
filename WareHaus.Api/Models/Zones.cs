using System;

namespace WareHaus.Api.Models;

public class Zones : BaseEntities
{
    public string ZoneCode { get; set; } = string.Empty;
    public string ZoneName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int TotalAisle { get; set; }
    public int ShelfPerAisle { get; set; }
    public int LevelPerShelf { get; set; }
}