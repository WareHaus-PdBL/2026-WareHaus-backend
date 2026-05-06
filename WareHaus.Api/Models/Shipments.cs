namespace WareHaus.Api.Models;

public class Shipments : BaseEntities
{
    public int PackingTaskId { get; set; }

    public string CourierName { get; set; } = "NONE";

    public string TrackingNumber { get; set; } = string.Empty;

    public string ShippingLabelUrl { get; set; } = string.Empty;

    public DateTime? ManifestDate { get; set; }

    public string Status { get; set; } = "Ready";

    public PackingTasks? PackingTasks { get; set; }
}