using System;

namespace WareHaus.Api.DTOs;

public record CreateReceivingDto(int POItemId, int QtyReceived, string Condition, DateTime ExpiryDate, string? PhotoUrl);
public record ReceivingResponseDto(int Id, int POItemId, int QtyReceived, string Condition, DateTime ReceivedAt, DateTime ExpiryDate, string PhotoUrl);