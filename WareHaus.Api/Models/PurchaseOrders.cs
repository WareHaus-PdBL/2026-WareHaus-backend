namespace WareHaus.Api.Models;

public class PurchaseOrders : BaseEntities
{
    public string PONumber { get; set; } = string.Empty;

    public string SupplierName { get; set; } = string.Empty;

    public string Status { get; set; } = "Pending";

    public DateTime OrderDate { get; set; }

    public ICollection<POItems> POItems { get; set; } = new List<POItems>();

    public ICollection<ReceivingLogs> ReceivingLogs { get; set; } = new List<ReceivingLogs>();
}