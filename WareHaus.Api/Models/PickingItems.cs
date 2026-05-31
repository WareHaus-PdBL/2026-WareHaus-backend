namespace WareHaus.Api.Models;

public class PickingItems : BaseEntities
{
    public int PickingTaskId { get; set; }

    public PickingTasks? PickingTask { get; set; }

    public int ProductId { get; set; }

    public Products? Product { get; set; }

    public int ShelfId { get; set; }

    public Shelves? Shelf { get; set; }

    public int QtyToPick { get; set; }

    public int QtyPicked { get; set; }

    public string UnitOfMeasureSnapshot { get; set; } = string.Empty;

    public string LocationSuggestion { get; set; } = string.Empty;

    public bool IsShelfVerified { get; set; }

    public string? ScannedShelfQrCode { get; set; }

    public DateTime? VerifiedAt { get; set; }
}