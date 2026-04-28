using System.ComponentModel.DataAnnotations;

namespace WareHaus.Api.DTOs.Zone;

public class CreateZoneDto
{
    [Required]
    public string ZoneCode { get; set; } = string.Empty;

    [Required]
    public string ZoneName { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    [Required, Range(1, int.MaxValue, ErrorMessage = "Lorong harus lebih dari 0.")]
    public int TotalAisle { get; set; }

    [Required, Range(1, int.MaxValue, ErrorMessage = "Rak per lorong harus lebih dari 0.")]
    public int ShelfPerAisle { get; set; }

    public int CapacityPerShelf { get; set; }

    public string Description { get; set; } = string.Empty;
}