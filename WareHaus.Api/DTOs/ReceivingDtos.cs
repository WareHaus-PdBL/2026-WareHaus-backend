using System;
using Microsoft.AspNetCore.Http;

namespace WareHaus.Api.DTOs;

public record CreateReceivingDto(int POItemId, int QtyReceived, string Condition, DateTime ExpiryDate, IFormFile? Photo);

public record ReceivingResponseDto(int Id, int POItemId, int QtyReceived, string Condition, DateTime ReceivedAt, DateTime ExpiryDate, string PhotoUrl);