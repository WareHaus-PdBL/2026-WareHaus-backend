namespace WareHaus.Api.Models;

public class Shipments : BaseEntities
{
    public int PackingTaskId { get; set; }

    public PackingTasks? PackingTask { get; set; }

    public int SalesOrderId { get; set; }

    public SalesOrders? SalesOrder { get; set; }

    public string ShippingLabelNumber { get; set; } = string.Empty;

    public string CourierName { get; set; } = string.Empty;

    public string TrackingNumber { get; set; } = string.Empty;

    public string? ShippingLabelUrl { get; set; }

    public string CustomerNameSnapshot { get; set; } = string.Empty;

    public string ShippingAddressSnapshot { get; set; } = string.Empty;

    public DateTime? ManifestDate { get; set; }

    public string Status { get; set; } = OutboundStatus.Completed;
}