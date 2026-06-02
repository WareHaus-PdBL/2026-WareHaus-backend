using System;

namespace WareHaus.Api.Models;

public class Stocks : BaseEntities
{
    public int ShelfId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }

    public Shelves? Shelves { get; set; }
    public Products? Products { get; set; }
}