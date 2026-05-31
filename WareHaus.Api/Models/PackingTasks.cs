namespace WareHaus.Api.Models;

public class PackingTasks : BaseEntities
{
    public string PackingNumber { get; set; } = string.Empty;

    public int SalesOrderId { get; set; }

    public SalesOrders? SalesOrder { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public int TotalPackage { get; set; }

    public string PackingStatus { get; set; } = OutboundStatus.Queued;

    public ICollection<PackingItems> PackingItems { get; set; } = new List<PackingItems>();

    public ICollection<Shipments> Shipments { get; set; } = new List<Shipments>();
}