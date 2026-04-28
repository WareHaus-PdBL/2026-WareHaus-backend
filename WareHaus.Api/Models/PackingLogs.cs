namespace WareHaus.Api.Models;

public class PackingLogs
{
    public Guid Id { get; set; }

    public Guid SalesOrderId { get; set; }

    public Guid ProductId { get; set; }

    public int QuantityPacked { get; set; }

    public string PackedBy { get; set; } = string.Empty;

    public DateTime PackedAt { get; set; } = DateTime.UtcNow;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public SalesOrders SalesOrder { get; set; } = null!;

    public Products Product { get; set; } = null!;
}