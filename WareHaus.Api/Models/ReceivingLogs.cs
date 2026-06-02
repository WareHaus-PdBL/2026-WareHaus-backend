using System;

namespace WareHaus.Api.Models;

public class ReceivingLogs : BaseEntities
{
    public int POItemId { get; set; }
    public int QtyReceived { get; set; }
    public string Condition { get; set; } = string.Empty;
    public DateTime ReceivedAt { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string PhotoUrl { get; set; } = string.Empty;

    public POItems? POItems { get; set; }
}