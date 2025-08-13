using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.RequestModels.Query;
using SRPM_Services.Interfaces;

namespace SRPM_APIServices.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectResultController : ControllerBase
    {
        private readonly IProjectResultService _projectResultService;

        public ProjectResultController(IProjectResultService projectResultService)
        {
            _projectResultService = projectResultService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RQ_ProjectResult request)
        {
            try
            {
                var result = await _projectResultService.CreateAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Create failed: {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] RQ_ProjectResult request)
        {
            try
            {

                var result = await _projectResultService.UpdateAsync(request);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Update failed: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var result = await _projectResultService.GetByIdAsync(id);
                return result == null ? NotFound("ProjectResult not found") : Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"GetById failed: {ex.Message}");
            }
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetList([FromBody] Q_ProjectResult query)
        {
            try
            {
                var result = await _projectResultService.GetListAsync(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"GetList failed: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var success = await _projectResultService.DeleteAsync(id);
                return success ? Ok("Deleted") : NotFound("ProjectResult not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Delete failed: {ex.Message}");
            }
        }

        [HttpDelete("publish/{id}")]
        public async Task<IActionResult> DeleteResultPublish(Guid id)
        {
            try
            {
                var success = await _projectResultService.DeleteResultPublishAsync(id);
                return success ? Ok("Deleted") : NotFound("ResultPublish not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"DeleteResultPublish failed: {ex.Message}");
            }
        }
    }
}
