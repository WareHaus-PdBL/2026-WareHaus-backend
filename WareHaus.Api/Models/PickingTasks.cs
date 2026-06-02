namespace WareHaus.Api.Models;

public class PickingTasks : BaseEntities
{
    public string PickingNumber { get; set; } = string.Empty;

    public int SalesOrderId { get; set; }

    public SalesOrders? SalesOrder { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public int TotalItems { get; set; }

    public string PickingStatus { get; set; } = OutboundStatus.Queued;

    public ICollection<PickingItems> PickingItems { get; set; } = new List<PickingItems>();
}