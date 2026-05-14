namespace WareHaus.Api.Models;

public class StockLogs : BaseEntities
{
    public int ProductId { get; set; }
    public int ShelfId { get; set; }

    public string MovementType { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public int StockAfterMovement { get; set; }

    public Products? Products { get; set; }
    public Shelves? Shelves { get; set; }
}