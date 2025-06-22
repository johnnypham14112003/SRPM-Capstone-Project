using Microsoft.AspNetCore.Mvc;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SRPM_APIServices.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _service;

        public ProjectController(IProjectService service)
        {
            _service = service;
        }

        // GET: api/project
        [HttpGet]
        public async Task<ActionResult<List<RS_Project>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        // GET: api/project/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<RS_Project>> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
                return NotFound($"Project with ID {id} not found.");
            return Ok(result);
        }

        // POST: api/project
        [HttpPost]
        public async Task<ActionResult<RS_Project>> Create(RQ_Project request)
        {
            var created = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/project/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<RS_Project>> Update(Guid id, RQ_Project request)
        {
            var updated = await _service.UpdateAsync(id, request);
            if (updated == null)
                return NotFound($"Project with ID {id} not found.");
            return Ok(updated);
        }

        // DELETE: api/project/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
                return NotFound($"Project with ID {id} not found.");
            return NoContent();
        }
    }

}
