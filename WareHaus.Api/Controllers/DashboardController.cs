using Microsoft.AspNetCore.Mvc;
using WareHaus.Api.DTOs;
using WareHaus.Api.Services;

namespace WareHaus.Api.Controllers;

[ApiController]
[Route("v1/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly DashboardService _dashboardService;

    public DashboardController(DashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("recent-logs")]
    public async Task<ActionResult<List<RecentStockLogDto>>> GetRecentStockLogsAsync(
        [FromQuery] int limit = 10
    )
    {
        var logs = await _dashboardService.GetRecentStockLogsAsync(limit);
        return Ok(logs);
    }
}