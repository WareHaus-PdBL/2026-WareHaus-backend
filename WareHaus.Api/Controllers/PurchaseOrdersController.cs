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
}