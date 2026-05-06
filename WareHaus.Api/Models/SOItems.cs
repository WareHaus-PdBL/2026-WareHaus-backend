namespace WareHaus.Api.Models;

public class SOItems : BaseEntities
{
    public int SOId { get; set; }

    public int ProductId { get; set; }

    public int QtyOrdered { get; set; }

    public int QtyPicked { get; set; }

    public SalesOrders? SalesOrders { get; set; }

    public Products? Products { get; set; }
}