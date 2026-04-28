namespace WareHaus.Api.Models;

public class PurchaseOrders : BaseEntities
{
    public string PONumber { get; set; } = string.Empty;

    public string SupplierName { get; set; } = string.Empty;

    public ICollection<POItems> POItems { get; set; } = new List<POItems>();
}