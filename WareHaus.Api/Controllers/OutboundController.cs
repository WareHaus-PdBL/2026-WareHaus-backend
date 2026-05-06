using Microsoft.AspNetCore.Mvc;
using WareHaus.Api.DTOs;
using WareHaus.Api.Services;

namespace WareHaus.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OutboundController : ControllerBase
{
    private readonly OutboundService _outboundService;

    public OutboundController(OutboundService outboundService)
    {
        _outboundService = outboundService;
    }

    [HttpGet("sales-orders")]
    public async Task<IActionResult> GetSalesOrders()
    {
        var result = await _outboundService.GetSalesOrdersAsync();

        return Ok(result);
    }

    [HttpGet("sales-orders/{id:int}")]
    public async Task<IActionResult> GetSalesOrderById(int id)
    {
        var result = await _outboundService.GetSalesOrderByIdAsync(id);

        return Ok(result);
    }

    [HttpPost("sales-orders")]
    public async Task<IActionResult> CreateSalesOrder([FromBody] CreateSalesOrderDto dto)
    {
        var result = await _outboundService.CreateSalesOrderAsync(dto);

        return Ok(result);
    }

    [HttpPut("sales-orders/{id:int}")]
    public async Task<IActionResult> UpdateSalesOrder(int id, [FromBody] UpdateSalesOrderDto dto)
    {
        var result = await _outboundService.UpdateSalesOrderAsync(id, dto);

        return Ok(result);
    }

    [HttpDelete("sales-orders/{id:int}")]
    public async Task<IActionResult> DeleteSalesOrder(int id)
    {
        await _outboundService.DeleteSalesOrderAsync(id);

        return NoContent();
    }

    [HttpPost("packing-tasks")]
    public async Task<IActionResult> CreatePackingTask([FromBody] CreatePackingTaskDto dto)
    {
        var result = await _outboundService.CreatePackingTaskAsync(dto);

        return Ok(result);
    }

    [HttpPost("packing-items/verify")]
    public async Task<IActionResult> VerifyPackingItem([FromBody] VerifyPackingItemDto dto)
    {
        await _outboundService.VerifyPackingItemAsync(dto);

        return Ok(new
        {
            message = "Packing item berhasil diverifikasi"
        });
    }

    [HttpPut("packing-tasks/complete")]
    public async Task<IActionResult> CompletePackingTask([FromBody] CompletePackingTaskDto dto)
    {
        var result = await _outboundService.CompletePackingTaskAsync(dto);

        return Ok(result);
    }

    [HttpPost("shipments")]
    public async Task<IActionResult> CreateShipment([FromBody] CreateShipmentDto dto)
    {
        var result = await _outboundService.CreateShipmentAsync(dto);

        return Ok(result);
    }
}