using Microsoft.AspNetCore.Mvc;
using WareHaus.Api.DTOs;
using WareHaus.Api.Services;

namespace WareHaus.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OutboundsController : ControllerBase
{
    private readonly OutboundServices _outboundServices;

    public OutboundsController(OutboundServices outboundServices)
    {
        _outboundServices = outboundServices;
    }

    [HttpPost("sales-orders")]
    public async Task<IActionResult> CreateSalesOrder([FromBody] CreateSalesOrderDto dto)
    {
        var result = await _outboundServices.CreateSalesOrderAsync(dto);

        return Ok(result);
    }

    [HttpPost("packing")]
    public async Task<IActionResult> PackItem([FromBody] PackingDto dto)
    {
        var result = await _outboundServices.PackItemAsync(dto);

        return Ok(result);
    }

    [HttpPost("shipping")]
    public async Task<IActionResult> ShipOrder([FromBody] ShippingDto dto)
    {
        var result = await _outboundServices.ShipOrderAsync(dto);

        return Ok(result);
    }
}