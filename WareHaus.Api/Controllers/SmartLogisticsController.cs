using Microsoft.AspNetCore.Mvc;
using WareHaus.Api.Services;
using System.Threading.Tasks;

namespace WareHaus.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SmartLogisticsController : ControllerBase
{
    private readonly SmartLogisticsService _smartService;

    public SmartLogisticsController(SmartLogisticsService smartService)
    {
        _smartService = smartService;
    }

    [HttpGet("put-away/{productId}")]
    public async Task<IActionResult> GetPutAway(int productId)
    {
        var result = await _smartService.GetPutAwayRecommendations(productId);
        return Ok(result);
    }

    [HttpGet("picking/{productId}")]
    public async Task<IActionResult> GetPicking(int productId)
    {
        var result = await _smartService.GetPickingRecommendations(productId);
        return Ok(result);
    }
}