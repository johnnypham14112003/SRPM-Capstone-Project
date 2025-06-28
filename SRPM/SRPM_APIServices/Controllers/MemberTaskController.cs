using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.BusinessModels;
using SRPM_Services.Interfaces;

namespace SRPM_APIServices.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MemberTaskController : ControllerBase
    {
        private readonly IMemberTaskService _service;

        public MemberTaskController(IMemberTaskService service)
        {
            _service = service;
        }

        // GET: api/membertask/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<RS_MemberTask>> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
                return NotFound($"MemberTask with ID {id} not found.");
            return Ok(result);
        }

        // GET: api/membertask/filter
        [HttpGet("filter")]
        public async Task<ActionResult<PagingResult<RS_MemberTask>>> GetList([FromQuery] RQ_MemberTaskQuery query)
        {
            var result = await _service.GetListAsync(query);
            return Ok(result);
        }

        // POST: api/membertask
        [HttpPost]
        public async Task<ActionResult<RS_MemberTask>> Create([FromBody] RQ_MemberTask request)
        {
            var created = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/membertask/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<RS_MemberTask>> Update(Guid id, [FromBody] RQ_MemberTask request)
        {
            var updated = await _service.UpdateAsync(id, request);
            if (updated == null)
                return NotFound($"MemberTask with ID {id} not found.");
            return Ok(updated);
        }

        // PUT: api/membertask/{id}/toggle-status
        [HttpPut("{id}/toggle-status")]
        public async Task<ActionResult<RS_MemberTask>> ToggleStatus(Guid id)
        {
            var toggled = await _service.ToggleStatusAsync(id);
            if (toggled == null)
                return NotFound($"MemberTask with ID {id} not found.");
            return Ok(toggled);
        }

        // DELETE: api/membertask/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success)
                return NotFound($"MemberTask with ID {id} not found.");
            return NoContent();
        }
    }

}
