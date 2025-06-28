using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.Interfaces;

namespace SRPM_APIServices.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectMajorController : ControllerBase
    {
        private readonly IProjectMajorService _service;

        public ProjectMajorController(IProjectMajorService service)
        {
            _service = service;
        }

        // GET: api/projectmajor/by-project/{projectId}
        [HttpGet("by-project/{projectId}")]
        public async Task<ActionResult<List<RS_ProjectMajor>>> GetByProject(Guid projectId)
        {
            var result = await _service.GetMajorsByProjectIdAsync(projectId);
            return Ok(result);
        }

        // POST: api/projectmajor
        [HttpPost]
        public async Task<ActionResult<RS_ProjectMajor>> Create(RQ_ProjectMajor request)
        {
            var created = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetByProject), new { projectId = created.ProjectId }, created);
        }

        // PUT: api/projectmajor/{projectId}/{majorId}
        [HttpPut("{projectId}/{majorId}")]
        public async Task<ActionResult<RS_ProjectMajor>> Update(Guid projectId, Guid majorId, RQ_ProjectMajor request)
        {
            var updated = await _service.UpdateAsync(projectId, majorId, request);
            if (updated == null)
                return NotFound($"No link found for ProjectId {projectId} and MajorId {majorId}.");

            return Ok(updated);
        }

        // DELETE: api/projectmajor/{projectId}/{majorId}
        [HttpDelete("{projectId}/{majorId}")]
        public async Task<IActionResult> Delete(Guid projectId, Guid majorId)
        {
            var success = await _service.DeleteAsync(projectId, majorId);
            if (!success)
                return NotFound($"No link found for ProjectId {projectId} and MajorId {majorId}.");

            return NoContent();
        }
    }

}
