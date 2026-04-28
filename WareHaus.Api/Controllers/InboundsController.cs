using Microsoft.AspNetCore.Mvc;
using WareHaus.Api.Services;
using WareHaus.Api.DTOs;
using System.Threading.Tasks;

namespace WareHaus.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InboundsController : ControllerBase
{
    private readonly InboundServices _inboundServices;

    public InboundsController(InboundServices inboundServices)
    {
        _inboundServices = inboundServices;
    }

    [HttpPost("purchase-orders")]
    public async Task<IActionResult> CreatePO([FromBody] CreatePurchaseOrderDto dto)
    {
        var result = await _inboundServices.CreatePOAsync(dto);
        return Ok(result);
    }

    [HttpPost("receiving")]
    public async Task<IActionResult> Receive([FromBody] ReceiveItemDto dto)
    {
        var result = await _inboundServices.ReceiveItemAsync(dto);
        return Ok(result);
    }

    [HttpPost("putaway")]
    public async Task<IActionResult> Putaway([FromBody] PutawayDto dto)
    {
        var result = await _inboundServices.PutawayAsync(dto);
        return Ok(result);
    }
}