namespace WareHaus.Api.Models;

public class SalesOrders : BaseEntities
{
    public string SONumber { get; set; } = string.Empty;

    public string CustomerName { get; set; } = "Someone";

    public string Status { get; set; } = "Pending";

    public DateTime OrderDate { get; set; }

    public ICollection<SOItems> SOItems { get; set; } = new List<SOItems>();

    public ICollection<PackingTasks> PackingTasks { get; set; } = new List<PackingTasks>();
}