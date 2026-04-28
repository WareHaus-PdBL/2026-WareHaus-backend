namespace WareHaus.Api.Models;

public class POItems : BaseEntities
{
    public Guid PurchaseOrderId { get; set; }

    public Guid ProductId { get; set; }

    public int QtyExpected { get; set; }

    public int QtyReceived { get; set; }

    public PurchaseOrders PurchaseOrder { get; set; } = null!;

    public Products Product { get; set; } = null!;

    public ICollection<ReceivingLogs> ReceivingLogs { get; set; } = new List<ReceivingLogs>();
}