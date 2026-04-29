using System;
using System.Collections.Generic;

namespace WareHaus.Api.DTOs;

public record CreatePurchaseOrderDto(string PONumber, string SupplierName, List<CreatePOItemDto> Items);
public record CreatePOItemDto(Guid ProductId, int QtyExpected);

public record PurchaseOrderResponseDto(Guid Id, string PONumber, string SupplierName, string Status, List<POItemResponseDto> Items);
public record POItemResponseDto(Guid Id, Guid ProductId, int QtyExpected);