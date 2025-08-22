using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SRPM_Services.BusinessModels.Others;
using SRPM_Services.BusinessModels.RequestModels;
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
    public async Task<ActionResult> GetList([FromBody] RQ_ProjectQuery query)
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
    public async Task<ActionResult> Create(RQ_Project request)
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

    [HttpPost("document")]
    public async Task<IActionResult> CreateMilestoneTaskFromDocument(RQ_MilestoneTaskContent request)
    {
        var result = await _service.CreateFromDocumentAsync(request);
        return result? Ok("Generate Success!") : BadRequest("Failed to save data!");
    }

    // PUT: api/project/{id}
    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult> Update(Guid id, RQ_Project request, string status)
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
    public async Task<ActionResult> GetMyProject([FromQuery]List<string> Statuses, [FromQuery] List<string> Genres)
    {
        try
        {
            var result = await _service.GetAllOnlineUserProjectAsync(Statuses, Genres);
            return result?.Count > 0 ? Ok(result) : NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Unexpected error: {ex.Message}");
        }
    }

    [HttpGet("host")]
    public async Task<ActionResult> GetHostProjectHistory()
    {
        try
        {
            var result = await _service.GetHostProjectHistory();
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Unexpected error occurred.", detail = ex.Message });
        }
    }

    /// <summary>
    /// Get all projects proposed by the current Staff account.
    /// </summary>
    [HttpGet("staff")]
    public async Task<ActionResult> GetStaffProjectHistory()
    {
        try
        {
            var result = await _service.GetStaffProjectHistory();
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Unexpected error occurred.", detail = ex.Message });
        }
    }
    [HttpPost("{proposalProjectId}/approve")]
    public async Task<IActionResult> ApproveProposal(Guid proposalProjectId)
    {
        try
        {
            var success = await _service.ApproveProposalAsync(proposalProjectId);

            if (!success)
                return BadRequest("Proposal approval failed due to unknown reasons.");

            return Ok("Proposal approved successfully.");
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            // Optional: log the exception here
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    [HttpGet("check-enrollment/{sourceProjectId}")]
    public async Task<IActionResult> CheckEnrollment(Guid sourceProjectId)
    {
        try
        {
            var result = await _service.CheckIsEnrollInProject(sourceProjectId);

            return Ok(new
            {
                ProposalId = result.id,
                IsEnrolled = result.isEnrolled
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An error occurred.", Details = ex.Message });
        }
    }


}