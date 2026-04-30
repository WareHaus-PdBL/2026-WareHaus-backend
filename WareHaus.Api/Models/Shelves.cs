using System;
using System.Collections.Generic;

namespace WareHaus.Api.Models;

public class Shelves : BaseEntities
{
    public int ZoneId { get; set; }
    public string ShelfCode { get; set; } = string.Empty;
    public int Aisle { get; set; }
    public int Capacity { get; set; }
    public int CurrentVolume { get; set; }
    public string QRCodePath { get; set; } = string.Empty;

    public Zones? Zones { get; set; }
    public ICollection<Stocks> Stocks { get; set; } = new List<Stocks>();
}