using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SRPM_Services.BusinessModels;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.Extensions.Exceptions;
using SRPM_Services.Interfaces;

namespace SRPM_APIServices.Controllers;

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
    // GET: api/userrole/filter
    [HttpPost("filter")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<PagingResult<RS_UserRole>>> GetList([FromBody] RQ_UserRoleQuery query)
    {
        var result = await _service.GetListAsync(query);
        return Ok(result);
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
        try
        {
            var created = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            // For unexpected errors
            return StatusCode(500, new { message = "An unexpected error occurred.", detail = ex.Message });
        }
    }

    // PUT: api/userrole/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<RS_UserRole>> Update(Guid id, RQ_UserRole request, string? Status)
    {
        try
        {
            var updated = await _service.UpdateAsync(id, request, Status);
            if (updated == null)
                return NotFound($"UserRole with ID {id} not found.");
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Failed to update UserRole.", detail = ex.Message });
        }
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