namespace WareHaus.Api.Models;

public class StockLogs : BaseEntities
{
    public int StockId { get; set; }
    public String status { get; set; } = string.Empty;
    public int QuantityChange { get; set; }
    public DateTime Timestamp { get; set; }

    public Shelves? Shelves { get; set; }
    public Products? Products { get; set; }
}