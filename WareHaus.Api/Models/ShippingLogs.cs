namespace WareHaus.Api.Models;

public class ShippingLogs
{
    public Guid Id { get; set; }

    public Guid SalesOrderId { get; set; }

    public string CourierName { get; set; } = string.Empty;

    public string TrackingNumber { get; set; } = string.Empty;

    public string ShippingStatus { get; set; } = "Shipped";

    public DateTime ShippedAt { get; set; } = DateTime.UtcNow;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public SalesOrders SalesOrder { get; set; } = null!;
}