using Microsoft.AspNetCore.Mvc;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.Interfaces;

namespace SRPM_APIServices.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectTagController : ControllerBase
{
    private readonly IProjectTagService _service;

    public ProjectTagController(IProjectTagService service)
    {
        _service = service;
    }

    // GET: api/projecttag/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<RS_ProjectTag>> GetById(Guid id)
    {
        var tag = await _service.GetByIdAsync(id);
        if (tag == null)
            return NotFound($"ProjectTag with ID {id} not found.");
        return Ok(tag);
    }

    // GET: api/projecttag/project/{projectId}
    [HttpGet("project/{projectId}")]
    public async Task<ActionResult<List<RS_ProjectTag>>> GetByProjectId(Guid projectId)
    {
        var tags = await _service.GetByProjectIdAsync(projectId);
        return Ok(tags);
    }

    [HttpPost]
    public async Task<IActionResult> Create(RQ_ProjectTag request)
    {
        try
        {
            var createdTags = await _service.CreateAsync(request);
            return Ok(createdTags);
        }
        catch (Exception ex)
        {
            // Optionally log the error here if you have a logger
            return BadRequest(new { message = ex.Message });
        }
    }
    // PUT: api/projecttag/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<RS_ProjectTag>> Update(Guid id, RQ_ProjectTag request)
    {
        var updated = await _service.UpdateAsync(id, request);
        if (updated == null)
            return NotFound($"ProjectTag with ID {id} not found.");
        return Ok(updated);
    }

    // DELETE: api/projecttag/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await _service.DeleteAsync(id);
        if (!success)
            return NotFound($"ProjectTag with ID {id} not found.");
        return NoContent();
    }
}