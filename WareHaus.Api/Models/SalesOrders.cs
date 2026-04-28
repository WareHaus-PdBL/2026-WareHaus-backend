namespace WareHaus.Api.Models;

public class SalesOrders
{
    public Guid Id { get; set; }

    public string SONumber { get; set; } = string.Empty;

    public string CustomerName { get; set; } = string.Empty;

    public string Status { get; set; } = "Created";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public ICollection<SOItems> SOItems { get; set; } = new List<SOItems>();

    public ICollection<PackingLogs> PackingLogs { get; set; } = new List<PackingLogs>();

    public ICollection<ShippingLogs> ShippingLogs { get; set; } = new List<ShippingLogs>();
}