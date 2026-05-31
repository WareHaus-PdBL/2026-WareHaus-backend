namespace WareHaus.Api.Models;

public class PackingItems : BaseEntities
{
    public int PackingTaskId { get; set; }

    public PackingTasks? PackingTask { get; set; }

    public int ProductId { get; set; }

    public Products? Product { get; set; }

    public int QtyExpected { get; set; }

    public int QtyVerified { get; set; }

    public string ExpectedBarcode { get; set; } = string.Empty;

    public string? ScannedBarcode { get; set; }

    public bool IsVerified { get; set; }

    public DateTime? VerifiedAt { get; set; }
}