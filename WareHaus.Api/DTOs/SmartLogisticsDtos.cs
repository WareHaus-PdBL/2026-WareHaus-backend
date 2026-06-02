using System;

namespace WareHaus.Api.DTOs;

public record PutAwayRecommendationDto(int ShelfId, string ShelfCode, string ZoneName, int AvailableCapacity);

public record PickingRecommendationDto(int Id, string LocationName, int StockAvailable, DateTime? ExpiryDate);