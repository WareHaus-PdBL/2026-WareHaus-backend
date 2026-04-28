namespace WareHaus.Api.Models;

public class Products : BaseEntities
{
    public string SKU { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public string? Barcode { get; set; }

    public string UnitOfMeasure { get; set; } = string.Empty;

    public ICollection<POItems> POItems { get; set; } = new List<POItems>();

    public ICollection<Stocks> Stocks { get; set; } = new List<Stocks>();
}