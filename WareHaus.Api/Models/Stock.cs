namespace WareHaus.API.Models;

public class Stock
{
    public int ShelfId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public Shelf? Shelf { get; set; }

    public Product? Product { get; set; }
}