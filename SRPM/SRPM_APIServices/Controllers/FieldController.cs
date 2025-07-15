using Microsoft.AspNetCore.Mvc;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.BusinessModels;
using SRPM_Services.Interfaces;

namespace SRPM_APIServices.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FieldController : ControllerBase
{
    private readonly IFieldService _service;

    public FieldController(IFieldService service)
    {
        _service = service;
    }

    // GET: api/field/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<RS_Field>> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound($"Field with ID {id} not found.");
        return Ok(result);
    }

    // GET: api/field/filter?name=ai&status=created&pageIndex=1&pageSize=10
    [HttpGet("filter")]
    public async Task<ActionResult<PagingResult<RS_Field>>> GetList(
        [FromQuery] string? name,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _service.GetListAsync(name, pageIndex, pageSize);
        return Ok(result);
    }

    // POST: api/field
    [HttpPost]
    public async Task<ActionResult<RS_Field>> Create([FromBody] RQ_Field request)
    {
        var created = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT: api/field/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<RS_Field>> Update(Guid id, [FromBody] RQ_Field request)
    {
        var updated = await _service.UpdateAsync(id, request);
        if (updated == null) return NotFound($"Field with ID {id} not found.");
        return Ok(updated);
    }


    // DELETE: api/field/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await _service.DeleteAsync(id);
        if (!success) return NotFound($"Field with ID {id} not found.");
        return NoContent();
    }
    // GET: api/field/all?name=engineering
    [HttpGet("all")]
    public async Task<ActionResult<List<RS_Field>>> GetAll()
    {
        var result = await _service.GetAllAsync();

        if (result == null)
            return NotFound("No fields found matching the criteria.");

        return Ok(result);
    }
}