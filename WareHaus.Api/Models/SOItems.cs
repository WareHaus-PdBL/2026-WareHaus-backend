namespace WareHaus.Api.Models;

public class SOItems
{
    public Guid Id { get; set; }

    public Guid SalesOrderId { get; set; }

    public Guid ProductId { get; set; }

    public int Quantity { get; set; }

    public int PackedQuantity { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public SalesOrders SalesOrder { get; set; } = null!;

    public Products Product { get; set; } = null!;
}