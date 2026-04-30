
using Microsoft.AspNetCore.Mvc;
using WareHaus.Api.DTOs;
using WareHaus.Api.Services;

namespace WareHaus.Api.Controllers;

[ApiController]
[Route("v1/[controller]")]
public class ZoneController : ControllerBase
{
    private readonly IZoneService _zoneService;

    public ZoneController(IZoneService zoneService)
    {
        _zoneService = zoneService;
    }

    [HttpGet]
    public async Task<ActionResult<List<GetAllZonesDto>>> GetAllZones()
    {
        var zones = await _zoneService.GetAllZonesAsync();
        return Ok(zones);
    }

    [HttpGet("{zoneId}")]
    public async Task<ActionResult<GetDetailsZoneDto>> GetZoneDetails(int zoneId)
    {
        var zoneDetails = await _zoneService.GetDetailsZoneAsync(zoneId, null);
        return Ok(zoneDetails);
    }

    [HttpGet("{zoneId}/{aisle}")]
    public async Task<ActionResult<GetDetailsZoneDto>> GetZoneDetailsByAisle(int zoneId, int aisle)
    {
        var zoneDetails = await _zoneService.GetDetailsZoneAsync(zoneId, aisle);
        return Ok(zoneDetails);
    }

    [HttpPost]
    public async Task<ActionResult<GetAllZonesDto>> CreateZone(CreateZoneDto createZoneDto)
    {
        var createdZone = await _zoneService.CreateZoneAsync(createZoneDto);
        return CreatedAtAction(nameof(GetZoneDetails), new { zoneId = createdZone.Id }, createdZone);
    }

    [HttpPut("{zoneId}")]
    public async Task<ActionResult<GetAllZonesDto>> UpdateZone(int zoneId, UpdateZoneDto updateZoneDto)
    {
        var updatedZone = await _zoneService.UpdateZoneAsync(zoneId, updateZoneDto);
        if (updatedZone == null)
        {
            return NotFound();
        }
        return Ok(updatedZone);
    }

    [HttpDelete("{zoneId}")]
    public async Task<IActionResult> DeleteZone(int zoneId)
    {
        await _zoneService.DeleteZoneAsync(zoneId);
        return NoContent();
    }
}