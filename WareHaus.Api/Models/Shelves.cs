using System;
using System.Collections.Generic;

namespace WareHaus.Api.Models;

public class Shelves
{
    public int Id { get; set; }
    public string ShelfCode { get; set; } = string.Empty;
    public int ZoneId { get; set; }
    
    public int Aisle { get; set; } 
    public int Capacity { get; set; }
    public int CurrentVolume { get; set; }
    public string? QRCodePath { get; set; }

    public int MaxCapacity { get; set; }
    public int CurrentCapacity { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public Zones? Zones { get; set; }
    
    public ICollection<Stocks> Stocks { get; set; } = new List<Stocks>();
}