using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.Interfaces;

namespace SRPM_APIServices.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserRoleController : ControllerBase
    {
        private readonly IUserRoleService _service;

        public UserRoleController(IUserRoleService service)
        {
            _service = service;
        }

        // GET: api/userrole
        [HttpGet]
        public async Task<ActionResult<List<RS_UserRole>>> GetAll()
        {
            var roles = await _service.GetAllAsync();
            return Ok(roles);
        }

        // GET: api/userrole/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<RS_UserRole>> GetById(Guid id)
        {
            var role = await _service.GetByIdAsync(id);
            if (role == null)
                return NotFound($"UserRole with ID {id} not found.");
            return Ok(role);
        }

        // POST: api/userrole
        [HttpPost]
        public async Task<ActionResult<RS_UserRole>> Create(RQ_UserRole request)
        {
            var created = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/userrole/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<RS_UserRole>> Update(Guid id, RQ_UserRole request)
        {
            var updated = await _service.UpdateAsync(id, request);
            if (updated == null)
                return NotFound($"UserRole with ID {id} not found.");
            return Ok(updated);
        }

        // PUT: api/userrole/{id}/toggle-status
        [HttpPut("{id}/toggle-status")]
        public async Task<ActionResult<RS_UserRole>> ToggleStatus(Guid id)
        {
            var result = await _service.ToggleStatusAsync(id);
            if (result == null)
                return NotFound($"UserRole with ID {id} not found.");
            return Ok(result);
        }

        // DELETE: api/userrole/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
                return NotFound($"UserRole with ID {id} not found.");
            return NoContent();
        }
    }

}
