using System;
using System.Collections.Generic;

namespace WareHaus.Api.DTOs;

public record CreatePurchaseOrderDto(string? PONumber, string SupplierName, List<CreatePOItemDto> Items);
public record CreatePOItemDto(int ProductId, int QtyExpected);

public record PurchaseOrderResponseDto(int Id, string PONumber, string SupplierName, string Status, int TotalQtyExpected, int TotalQtyReceived, List<POItemResponseDto> Items);

public record POItemResponseDto(int Id, int ProductId, int QtyExpected, int QtyReceived);