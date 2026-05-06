namespace WareHaus.Api.Models;

public class ReceivingLogs : BaseEntities
{
    public int PurchaseOrderId { get; set; }

    public int POItemId { get; set; }

    public int ProductId { get; set; }

    public int QtyReceived { get; set; }

    public string Condition { get; set; } = string.Empty;

    public DateTime? ExpiryDate { get; set; }

    public DateTime ReceivedAt { get; set; }

    public PurchaseOrders? PurchaseOrders { get; set; }

    public POItems? POItems { get; set; }

    public Products? Products { get; set; }
}