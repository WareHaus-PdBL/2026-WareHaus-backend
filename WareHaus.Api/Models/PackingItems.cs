namespace WareHaus.Api.Models;

public class PackingItems : BaseEntities
{
    public int PackingTaskId { get; set; }

    public int ProductId { get; set; }

    public int QtyVerified { get; set; }

    public PackingTasks? PackingTasks { get; set; }

    public Products? Products { get; set; }
}