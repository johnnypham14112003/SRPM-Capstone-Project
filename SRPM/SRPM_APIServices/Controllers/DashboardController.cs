using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SRPM_Services.Interfaces;

namespace SRPM_APIServices.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("system-stats")]
        public async Task<IActionResult> GetSystemStats([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var start = from ?? DateTime.MinValue;
            var end = to ?? DateTime.Now;
            var result = await _dashboardService.GetSystemStatsAsync(start, end);
            return Ok(result);
        }

        [HttpGet("transactions")]
        public async Task<IActionResult> GetTransactionStats([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var start = from ?? DateTime.MinValue;
            var end = to ?? DateTime.Now;
            var result = await _dashboardService.GetTransactionStatsAsync(start, end);
            return Ok(result);
        }

        [HttpGet("milestones/progress")]
        public async Task<IActionResult> GetMilestoneProgressStats()
        {
            var result = await _dashboardService.GetMilestoneProgressStatsAsync();
            return Ok(result);
        }

        [HttpGet("councils")]
        public async Task<IActionResult> GetCouncilProjectStats()
        {
            var result = await _dashboardService.GetCouncilProjectStatsAsync();
            return Ok(result);
        }

        [HttpGet("projects/status")]
        public async Task<IActionResult> GetSourceProjectStatusStats([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var start = from ?? DateTime.MinValue;
            var end = to ?? DateTime.Now;
            var result = await _dashboardService.GetSourceProjectStatusStatsAsync(start, end);
            return Ok(result);
        }

        [HttpGet("userroles/base")]
        public async Task<IActionResult> GetBaseUserRoleDistribution()
        {
            var result = await _dashboardService.GetBaseUserRoleDistributionAsync();
            return Ok(result);
        }

        [HttpGet("timeseries")]
        public async Task<IActionResult> GetTimeSeriesStats(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] string granularity = "daily")
        {
            var end = to ?? DateTime.Now;
            var result = await _dashboardService.GetTimeSeriesStatsAsync(from, end, granularity);
            return Ok(result);
        }

        [HttpGet("majors/distribution")]
        public async Task<IActionResult> GetMajorDistributionStats([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var start = from ?? DateTime.MinValue;
            var end = to ?? DateTime.Now;
            var result = await _dashboardService.GetMajorDistributionStatsAsync(start, end);
            return Ok(result);
        }

    }

}
