namespace WareHaus.Api.Models;

public class PackingTasks : BaseEntities
{
    public int SOId { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public int TotalPackage { get; set; }

    public string PackingStatus { get; set; } = "Pending";

    public SalesOrders? SalesOrders { get; set; }

    public ICollection<PackingItems> PackingItems { get; set; } = new List<PackingItems>();

    public ICollection<Shipments> Shipments { get; set; } = new List<Shipments>();
}