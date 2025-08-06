using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SRPM_Services.BusinessModels;
using SRPM_Services.BusinessModels.Others;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.Extensions.Exceptions;
using SRPM_Services.Implements;
using SRPM_Services.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SRPM_APIServices.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectController : Controller
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
    public async Task<ActionResult<object>> GetById(Guid id)
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
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.Id);
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
    public async Task<ActionResult<RS_Project>> Update(Guid id, RQ_Project request, string status)
    {
        try
        {
            var updated = await _service.UpdateAsync(id, request, status);
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
    // POST: api/project/enroll-as-principal/{sourceProjectId}
    [HttpPost("enroll-as-principal/{sourceProjectId}")]
    public async Task<IActionResult> EnrollAsPrincipal(Guid sourceProjectId)
    {
        try
        {
            var enrolledProject = await _service.EnrollAsPrincipalAsync(sourceProjectId);
            return Ok(enrolledProject);
        }
        catch (NotFoundException nfEx)
        {
            return NotFound(new { message = nfEx.Message });
        }
        catch (UnauthorizedAccessException uaEx)
        {
            return StatusCode(403, new { message = uaEx.Message });
        }
        catch (InvalidOperationException ioEx)
        {
            return Conflict(new { message = ioEx.Message });
        }
        catch (Exception ex)
        {
            // You can log the exception here if needed
            return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
        }
    }


    // GET: api/project/overview
    [HttpGet("my-project")]
    [Authorize]
    public async Task<ActionResult<List<RS_ProjectOverview>>> GetMyProject()
    {
        try
        {
            var result = await _service.GetAllOnlineUserProjectAsync();
            return result?.Count > 0 ? Ok(result) : NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Unexpected error: {ex.Message}");
        }
    }

}