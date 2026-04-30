namespace WareHaus.Api.Models;

public class Product : BaseEntity
{
    public string SKU { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public string Barcode { get; set; } = string.Empty;

    public string UnitOfMeasure { get; set; } = string.Empty;

    public ICollection<Stock> Stocks { get; set; } = new List<Stock>();
}