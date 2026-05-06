namespace WareHaus.Api.Models;

public class Stocks : BaseEntities
{
    public int ProductId { get; set; }

    public int ShelfId { get; set; }

    public int Quantity { get; set; }

    public Products? Products { get; set; }

    public Shelves? Shelves { get; set; }
}