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
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _service;

        public TaskController(ITaskService service)
        {
            _service = service;
        }

        // GET: api/task/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<RS_Task>> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
                return NotFound($"Task with ID {id} not found.");
            return Ok(result);
        }

        // GET: api/task/filter
        [HttpGet("filter")]
        public async Task<ActionResult<PagingResult<RS_Task>>> GetList([FromQuery] RQ_TaskQuery query)
        {
            var result = await _service.GetListAsync(query);
            return Ok(result);
        }

        // POST: api/task
        [HttpPost]
        public async Task<ActionResult<RS_Task>> Create([FromBody] RQ_Task request)
        {
            var created = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/task/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<RS_Task>> Update(Guid id, [FromBody] RQ_Task request)
        {
            var updated = await _service.UpdateAsync(id, request);
            if (updated == null)
                return NotFound($"Task with ID {id} not found.");
            return Ok(updated);
        }

        // PUT: api/task/{id}/toggle-status
        [HttpPut("{id}/toggle-status")]
        public async Task<ActionResult<RS_Task>> ToggleStatus(Guid id)
        {
            var result = await _service.ToggleStatusAsync(id);
            if (result == null)
                return NotFound($"Task with ID {id} not found.");
            return Ok(result);
        }

        // DELETE: api/task/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success)
                return NotFound($"Task with ID {id} not found.");
            return NoContent();
        }
    }

}
