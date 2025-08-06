using Microsoft.AspNetCore.Mvc;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.Interfaces;

namespace SRPM_APIServices.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoleController : Controller
{
    private readonly IRoleService _service;

    public RoleController(IRoleService service)
    {
        _service = service;
    }

    // GET: api/role
    [HttpGet]
    public async Task<ActionResult<List<RS_Role>>> GetAll()
    {
        var roles = await _service.GetAllAsync();
        return Ok(roles);
    }

    // GET: api/role/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<RS_Role>> GetById(Guid id)
    {
        var role = await _service.GetByIdAsync(id);
        if (role == null)
            return NotFound($"Role with ID {id} not found.");
        return Ok(role);
    }

    // POST: api/role
    [HttpPost]
    public async Task<ActionResult<RS_Role>> Create(RQ_Role request)
    {
        var created = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT: api/role/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<RS_Role>> Update(Guid id, RQ_Role request)
    {
        var updated = await _service.UpdateAsync(id, request);
        if (updated == null)
            return NotFound($"Role with ID {id} not found.");
        return Ok(updated);
    }

    // PUT: api/role/{id}/toggle-status
    [HttpPut("{id}/toggle-status")]
    public async Task<ActionResult<RS_Role>> ToggleStatus(Guid id)
    {
        var toggled = await _service.ToggleStatusAsync(id);
        if (toggled == null)
            return NotFound($"Role with ID {id} not found.");
        return Ok(toggled);
    }

    // DELETE: api/role/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted)
            return NotFound($"Role with ID {id} not found.");
        return NoContent();
    }
}