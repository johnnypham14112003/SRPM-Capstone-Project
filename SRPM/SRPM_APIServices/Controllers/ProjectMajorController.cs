using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SRPM_Services.BusinessModels;
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

        // GET: api/projectmajor/filter
        [HttpGet("filter")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<PagingResult<RS_ProjectMajor>>> GetList([FromQuery] RQ_ProjectMajorQuery query)
        {
            var result = await _service.GetListAsync(query);
            return Ok(result);
        }



        // POST: api/projectmajor
        [HttpPost]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<RS_ProjectMajor>> Create([FromBody] RQ_ProjectMajor request)
        {
            var created = await _service.CreateAsync(request);

            // Redirect to filtered list with ProjectId and MajorId as route values
            return CreatedAtAction(
                nameof(GetList),
                new { projectId = created.ProjectId, majorId = created.MajorId },
                created
            );
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
