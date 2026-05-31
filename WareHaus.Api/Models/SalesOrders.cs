namespace WareHaus.Api.Models;

public class SalesOrders : BaseEntities
{
    public string SONumber { get; set; } = string.Empty;

    public string CustomerName { get; set; } = string.Empty;

    public string ShippingAddress { get; set; } = string.Empty;

    public string Courier { get; set; } = string.Empty;

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public DateTime RequiredDeliveryDate { get; set; }

    public string? TrackingNumber { get; set; }

    public string Status { get; set; } = OutboundStatus.Queued;

    public ICollection<SOItems> SOItems { get; set; } = new List<SOItems>();

    public ICollection<PickingTasks> PickingTasks { get; set; } = new List<PickingTasks>();

    public ICollection<PackingTasks> PackingTasks { get; set; } = new List<PackingTasks>();

    public ICollection<Shipments> Shipments { get; set; } = new List<Shipments>();
}