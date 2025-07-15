using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SRPM_Services.BusinessModels;
using SRPM_Services.BusinessModels.Others;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.Implements;
using SRPM_Services.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SRPM_APIServices.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectController : ControllerBase
{
    private readonly IProjectService _service;

    public ProjectController(IProjectService service)
    {
        _service = service;
    }

    // GET: api/project/filter
    [HttpPost("filter")]
    [Authorize]
    public async Task<ActionResult<PagingResult<RS_Project>>> GetList([FromBody] RQ_ProjectQuery query)
    {
        try
        {
            var result = await _service.GetListAsync(query);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Unexpected error: {ex.Message}");
        }
    }

    // GET: api/project/{id}
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<RS_Project>> GetById(Guid id)
    {
        try
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
                return NotFound($"Project with ID {id} not found.");

            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Unexpected error: {ex.Message}");
        }
    }

    // POST: api/project
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<RS_Project>> Create(RQ_Project request)
    {
        try
        {
            var created = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Unexpected error: {ex.Message}");
        }
    }

    // PUT: api/project/{id}
    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<RS_Project>> Update(Guid id, RQ_Project request)
    {
        try
        {
            var updated = await _service.UpdateAsync(id, request);
            if (updated == null)
                return NotFound($"Project with ID {id} not found.");

            return Ok(updated);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Unexpected error: {ex.Message}");
        }
    }

    // DELETE: api/project/{id}
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
                return NotFound($"Project with ID {id} not found.");

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Unexpected error: {ex.Message}");
        }
    }

    // GET: api/project/overview
    [HttpGet("overview")]
    [Authorize]
    public async Task<ActionResult<List<RS_ProjectOverview>>> GetOverview()
    {
        try
        {
            var result = await _service.GetAllOverviewsAsync();
            return result?.Count > 0 ? Ok(result) : NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Unexpected error: {ex.Message}");
        }
    }

}