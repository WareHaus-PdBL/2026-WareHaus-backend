using System;
using System.Collections.Generic;

namespace WareHaus.Api.Models;

public class POItems : BaseEntities
{
    public int POId { get; set; }
    public int ProductId { get; set; }
    public int QtyExpected { get; set; }

    public PurchaseOrders? PurchaseOrders { get; set; }
    public Products? Products { get; set; }
    public ICollection<ReceivingLogs> ReceivingLogs { get; set; } = new List<ReceivingLogs>();
}