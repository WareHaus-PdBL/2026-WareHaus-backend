using System;

namespace WareHaus.Api.Models;

public class Products : BaseEntities
{
    public string SKU { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public string UnitOfMeasure { get; set; } = string.Empty;

    public ICollection<Stocks> Stocks { get; set; } = new List<Stocks>();
}