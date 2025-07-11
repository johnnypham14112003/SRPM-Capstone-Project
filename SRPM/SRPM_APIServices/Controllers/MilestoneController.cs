using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.BusinessModels;
using SRPM_Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using SRPM_Services.BusinessModels.Others;
using SRPM_Repositories.Models;

namespace SRPM_APIServices.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MilestoneController : ControllerBase
    {
        private readonly IMilestoneService _service;

        public MilestoneController(IMilestoneService service)
        {
            _service = service;
        }

        // GET: api/milestone/{id}
        [HttpGet("{id}")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Staff, Member")]
        public async Task<ActionResult<RS_Milestone>> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
                return NotFound($"Milestone with ID {id} not found.");
            return Ok(result);
        }

        // GET: api/milestone/project/{projectId}
        [HttpGet("project/{projectId}")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Staff, Member")]
        public async Task<ActionResult<List<RS_Milestone>>> GetByProjectId(Guid projectId)
        {
            var result = await _service.GetByProjectAsync(projectId);
            return Ok(result);
        }

        // GET: api/milestone/filter
        [HttpPost("filter")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Staff")]
        public async Task<ActionResult<PagingResult<RS_Milestone>>> GetList([FromBody] RQ_MilestoneQuery query)
        {
            var result = await _service.GetListAsync(query);
            return Ok(result);
        }

        // POST: api/milestone
        [HttpPost]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Staff")]
        public async Task<ActionResult<RS_Milestone>> Create([FromBody] RQ_Milestone request)
        {
            var created = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/milestone/{id}
        [HttpPut("{id}")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Staff")]
        public async Task<ActionResult<RS_Milestone>> Update(Guid id, [FromBody] RQ_Milestone request)
        {
            var updated = await _service.UpdateAsync(id, request);
            if (updated == null)
                return NotFound($"Milestone with ID {id} not found.");
            return Ok(updated);
        }

        // PUT: api/milestone/{id}/toggle-status
        [HttpPut("{id}/toggle-status")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Staff")]
        public async Task<ActionResult<RS_Milestone>> ToggleStatus(Guid id)
        {
            var toggled = await _service.ToggleStatusAsync(id);
            if (toggled == null)
                return NotFound($"Milestone with ID {id} not found.");
            return Ok(toggled);
        }

        // DELETE: api/milestone/{id}
        [HttpDelete("{id}")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Staff")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success)
                return NotFound($"Milestone with ID {id} not found.");
            return NoContent();
        }
    }

}
