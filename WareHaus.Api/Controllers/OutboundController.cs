using Microsoft.AspNetCore.Mvc;
using WareHaus.Api.DTOs;
using WareHaus.Api.Services;

namespace WareHaus.Api.Controllers;

[ApiController]
[Route("api/v1/outbound")]
public class OutboundController : ControllerBase
{
    private readonly OutboundService _outboundService;

    public OutboundController(OutboundService outboundService)
    {
        _outboundService = outboundService;
    }

    // ==========================
    // SALES ORDER
    // ==========================

    [HttpGet("sales-orders")]
    public async Task<IActionResult> GetSalesOrdersAsync()
    {
        var result = await _outboundService.GetSalesOrdersAsync();

        return Ok(result);
    }

    [HttpGet("sales-orders/{id:int}")]
    public async Task<IActionResult> GetSalesOrderByIdAsync(int id)
    {
        var result = await _outboundService.GetSalesOrderByIdAsync(id);

        return Ok(result);
    }

    [HttpPost("sales-orders")]
    public async Task<IActionResult> CreateSalesOrderAsync([FromBody] CreateSalesOrderDto dto)
    {
        var result = await _outboundService.CreateSalesOrderAsync(dto);

        return Ok(result);
    }

    [HttpPut("sales-orders/{id:int}")]
    public async Task<IActionResult> UpdateSalesOrderAsync(int id, [FromBody] UpdateSalesOrderDto dto)
    {
        var result = await _outboundService.UpdateSalesOrderAsync(id, dto);

        return Ok(result);
    }

    [HttpDelete("sales-orders/{id:int}")]
    public async Task<IActionResult> DeleteSalesOrderAsync(int id)
    {
        await _outboundService.DeleteSalesOrderAsync(id);

        return NoContent();
    }

    // ==========================
    // PICKING
    // ==========================

    [HttpPost("picking-tasks")]
    public async Task<IActionResult> CreatePickingTaskAsync([FromBody] CreatePickingTaskDto dto)
    {
        var result = await _outboundService.CreatePickingTaskAsync(dto);

        return Ok(result);
    }

    [HttpGet("picking-tasks/{id:int}")]
    public async Task<IActionResult> GetPickingTaskByIdAsync(int id)
    {
        var result = await _outboundService.GetPickingTaskByIdAsync(id);

        return Ok(result);
    }

    [HttpPost("picking-items/verify-shelf")]
    public async Task<IActionResult> VerifyPickingShelfAsync([FromBody] VerifyPickingShelfDto dto)
    {
        await _outboundService.VerifyPickingShelfAsync(dto);

        return Ok(new
        {
            message = "Lokasi shelf berhasil diverifikasi."
        });
    }

    // ==========================
    // PACKING
    // ==========================

    [HttpPost("packing-tasks")]
    public async Task<IActionResult> CreatePackingTaskAsync([FromBody] CreatePackingTaskDto dto)
    {
        var result = await _outboundService.CreatePackingTaskAsync(dto);

        return Ok(result);
    }

    [HttpGet("packing-tasks/{id:int}")]
    public async Task<IActionResult> GetPackingTaskByIdAsync(int id)
    {
        var result = await _outboundService.GetPackingTaskByIdAsync(id);

        return Ok(result);
    }

    [HttpPost("packing-items/verify-barcode")]
    public async Task<IActionResult> VerifyPackingItemAsync([FromBody] VerifyPackingItemDto dto)
    {
        await _outboundService.VerifyPackingItemAsync(dto);

        return Ok(new
        {
            message = "Packing item berhasil diverifikasi."
        });
    }

    [HttpPut("packing-tasks/complete")]
    public async Task<IActionResult> CompletePackingTaskAsync([FromBody] CompletePackingTaskDto dto)
    {
        var result = await _outboundService.CompletePackingTaskAsync(dto);

        return Ok(result);
    }

    // ==========================
    // SHIPMENT / SHIPPING LABEL
    // ==========================

    [HttpPost("shipments")]
    public async Task<IActionResult> CreateShipmentAsync([FromBody] CreateShipmentDto dto)
    {
        var result = await _outboundService.CreateShipmentAsync(dto);

        return Ok(result);
    }

    [HttpGet("sales-orders/{salesOrderId:int}/shipping-label")]
    public async Task<IActionResult> GetShippingLabelAsync(int salesOrderId)
    {
        var result = await _outboundService.GetShippingLabelAsync(salesOrderId);

        return Ok(result);
    }
}