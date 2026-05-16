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

    [HttpGet("{zoneId:int}")]
    public async Task<ActionResult<GetDetailsZoneDto>> GetZoneDetails(int zoneId)
    {
        var zoneDetails = await _zoneService.GetDetailsZoneAsync(zoneId, null);
        return Ok(zoneDetails);
    }

    [HttpGet("{zoneId:int}/{aisle:int}")]
    public async Task<ActionResult<GetDetailsZoneDto>> GetZoneDetailsByAisle(int zoneId, int aisle)
    {
        var zoneDetails = await _zoneService.GetDetailsZoneAsync(zoneId, aisle);
        return Ok(zoneDetails);
    }

    [HttpGet("qrcodes/zones/{zoneId:int}/aisles/{aisle:int}/pdf")]
    public async Task<IActionResult> DownloadAisleQrCodesPdf(int zoneId, int aisle)
    {
        var pdfBytes = await _zoneService.DownloadAisleQrCodesPdfAsync(zoneId, aisle);

        var fileName = $"zone-{zoneId}-aisle-{aisle}-qrcodes.pdf";

        return File(pdfBytes, "application/pdf", fileName);
    }

    [HttpPost]
    public async Task<ActionResult<GetAllZonesDto>> CreateZone(CreateZoneDto createZoneDto)
    {
        var createdZone = await _zoneService.CreateZoneAsync(createZoneDto);
        return CreatedAtAction(nameof(GetZoneDetails), new { zoneId = createdZone.Id }, createdZone);
    }

    [HttpPut("{zoneId:int}")]
    public async Task<ActionResult<GetAllZonesDto>> UpdateZone(int zoneId, UpdateZoneDto updateZoneDto)
    {
        var updatedZone = await _zoneService.UpdateZoneAsync(zoneId, updateZoneDto);
        return Ok(updatedZone);
    }

    [HttpDelete("{zoneId:int}")]
    public async Task<IActionResult> DeleteZone(int zoneId)
    {
        await _zoneService.DeleteZoneAsync(zoneId);
        return NoContent();
    }

    [HttpGet("{zoneId:int}/shelves")]
    public async Task<ActionResult<List<GetShelfForZoneDto>>> GetShelvesInZone(int zoneId)
    {
        var shelves = await _zoneService.GetShelvesInZoneAsync(zoneId);
        return Ok(shelves);
    }

    [HttpGet("shelves/{shelfId:int}")]
    public async Task<ActionResult<GetShelfForZoneDetailsDto>> GetShelfDetails(int shelfId)
    {
        var shelfDetails = await _zoneService.GetShelfDetailsAsync(shelfId);
        return Ok(shelfDetails);
    }

    [HttpPost("qr/{shelfId:int}")]
public async Task<ActionResult> DownloadQRCodeShelf(
    int shelfId,
    [FromBody] DownloadQrRequestDto request)
{
    var result = await _zoneService.DownloadQRCodeShelfAsync(shelfId, request.Option);

    return File(result.Bytes, result.ContentType, result.FileName);
}

[HttpPost("qr/{zoneId:int}/{aisle:int}")]
public async Task<ActionResult> DownloadQRCodeAisle(
    int zoneId,
    int aisle,
    [FromBody] DownloadQrRequestDto request)
{
    var result = await _zoneService.DownloadQRCodeAisleAsync(zoneId, aisle, request.Option);

    return File(result.Bytes, result.ContentType, result.FileName);
}
}