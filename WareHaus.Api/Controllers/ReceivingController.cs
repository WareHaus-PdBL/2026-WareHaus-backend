using Microsoft.AspNetCore.Mvc;
using WareHaus.Api.Services;
using WareHaus.Api.DTOs;
using System.Threading.Tasks;

namespace WareHaus.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReceivingController : ControllerBase
{
    private readonly ReceivingService _receivingService;

    public ReceivingController(ReceivingService receivingService)
    {
        _receivingService = receivingService;
    }

    [HttpPost]
    public async Task<IActionResult> ReceiveItem([FromBody] CreateReceivingDto dto)
    {
        var result = await _receivingService.ReceiveItemAsync(dto);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllLogs()
    {
        var result = await _receivingService.GetAllReceivingLogsAsync();
        return Ok(result);
    }

    [HttpGet("poitem/{poItemId}")]
    public async Task<IActionResult> GetLogsByPOItem(int poItemId)
    {
        var result = await _receivingService.GetLogsByPOItemAsync(poItemId);
        return Ok(result);
    }
}