using System;
using System.Collections.Generic;

namespace WareHaus.Api.Models;

public class Shelves : BaseEntities
{
    public Guid ZoneId { get; set; }
    public string ShelfCode { get; set; } = string.Empty;
    public string Aisle { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public int CurrentVolume { get; set; }

    public Zones? Zones { get; set; }
    public ICollection<Stocks> Stocks { get; set; } = new List<Stocks>();
}