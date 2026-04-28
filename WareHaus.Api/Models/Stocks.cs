namespace WareHaus.Api.Models;

public class Stocks : BaseEntities
{
    public Guid ShelfId { get; set; }

    public Guid ProductId { get; set; }

    public int Quantity { get; set; }

    public Shelves Shelf { get; set; } = null!;

    public Products Product { get; set; } = null!;
}