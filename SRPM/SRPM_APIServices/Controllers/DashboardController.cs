using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SRPM_Services.Interfaces;

namespace SRPM_APIServices.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("system-stats")]
        [Authorize(Roles = "Administrator, Staff")]
        public async Task<IActionResult> GetSystemStats([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var result = await _dashboardService.GetSystemStatsAsync(from, to);
            return Ok(result);
        }

        [HttpGet("user-project-stats")]
        public async Task<IActionResult> GetUserProjectStats([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var result = await _dashboardService.GetUserProjectStatsAsync(from, to);
            return Ok(result);
        }

        [HttpGet("principal-dashboard")]
        [Authorize(Roles = "Principal Investigator")]
        public async Task<IActionResult> GetPrincipalDashboard([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var result = await _dashboardService.GetPrincipalDashboardAsync(from, to);
            return Ok(result);
        }

        [HttpGet("institution-dashboard")]
        [Authorize(Roles = "Host Institution")]
        public async Task<IActionResult> GetInstitutionDashboard([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var result = await _dashboardService.GetInstitutionDashboardAsync(from, to);
            return Ok(result);
        }

        [HttpGet("kpi-tiles")]
        public async Task<IActionResult> GetKpiTiles([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var result = await _dashboardService.GetKpiTilesAsync(from, to);
            return Ok(result);
        }
    }

}
