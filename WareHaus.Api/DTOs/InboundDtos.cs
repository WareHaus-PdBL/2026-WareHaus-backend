using System;
using System.Collections.Generic;

namespace WareHaus.Api.DTOs;

public record CreatePurchaseOrderDto(string PONumber, string SupplierName, List<CreatePOItemDto> Items);
public record CreatePOItemDto(Guid ProductId, int QtyExpected);
public record ReceiveItemDto(Guid POItemId, int QtyReceived, string Condition, DateTime ExpiryDate);
public record PutawayDto(string ShelfCode, Guid ProductId, int Quantity);