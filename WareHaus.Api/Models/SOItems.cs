namespace WareHaus.Api.Models;

public class SOItems : BaseEntities
{
    public int SalesOrderId { get; set; }

    public SalesOrders? SalesOrder { get; set; }

    public int ProductId { get; set; }

    public Products? Product { get; set; }

    public int QtyOrdered { get; set; }

    public int QtyPicked { get; set; }

    public int QtyVerified { get; set; }

    public string UnitOfMeasureSnapshot { get; set; } = string.Empty;
}