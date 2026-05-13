using Microsoft.AspNetCore.Mvc;
using WareHaus.Api.Services;
using WareHaus.Api.DTOs;
using System.Threading.Tasks;

namespace WareHaus.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PurchaseOrdersController : ControllerBase
{
    private readonly PurchaseOrderService _poService;

    public PurchaseOrdersController(PurchaseOrderService poService)
    {
        _poService = poService;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePO([FromBody] CreatePurchaseOrderDto dto)
    {
        var result = await _poService.CreatePOAsync(dto);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPOs()
    {
        var result = await _poService.GetAllPOsAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPODetails(int id)
    {
        var result = await _poService.GetPODetailsAsync(id);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdatePOStatus(int id, [FromBody] string status)
    {
        var result = await _poService.UpdatePOStatusAsync(id, status);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePO(int id)
    {
        var success = await _poService.DeletePOAsync(id);
        if (!success)
        {
            return NotFound();
        }
        return NoContent();
    }
}