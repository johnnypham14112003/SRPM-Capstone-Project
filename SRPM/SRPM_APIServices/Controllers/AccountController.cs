using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.BusinessModels;
using SRPM_Services.Interfaces;
using SRPM_Services.BusinessModels.Others;

namespace SRPM_APIServices.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _service;

    public AccountController(IAccountService service)
    {
        _service = service;
    }

    // GET: api/account/{id}
    [HttpGet("{id}")]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<RS_Account>> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound($"Account with ID {id} not found.");
        return Ok(result);
    }

    // GET: api/account/filter
    [HttpPost("filter")]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<PagingResult<RS_Account>>> GetList([FromBody] RQ_AccountQuery query)
    {
        var result = await _service.GetListAsync(query);
        return Ok(result);
    }

    // POST: api/account
    [HttpPost]
    public async Task<ActionResult<RS_Account>> Create([FromBody] RQ_Account request)
    {
        var created = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT: api/account/{id}
    [HttpPut("{id}")]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<RS_Account>> Update(Guid id, [FromBody] RQ_Account request)
    {
        var updated = await _service.UpdateAsync(id, request);
        if (updated == null) return NotFound($"Account with ID {id} not found.");
        return Ok(updated);
    }

    // PUT: api/account/{id}/toggle-status
    [HttpPut("{id}/toggle-status")]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<RS_Account>> ToggleStatus(Guid id)
    {
        var result = await _service.ToggleStatusAsync(id);
        if (result == null) return NotFound($"Account with ID {id} not found.");
        return Ok(result);
    }

    // DELETE: api/account/{id}
    [HttpDelete("{id}")]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> Delete(Guid id)
    {
        var success = await _service.DeleteAsync(id);
        if (!success) return NotFound($"Account with ID {id} not found.");
        return NoContent();
    }
    // GET: api/account/me
    [HttpGet("me")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<RS_Account>> GetOnlineUserInfo()
    {
        var result = await _service.GetOnlineUserInfoAsync();
        if (result == null)
            return NotFound("Authenticated user not found.");

        return Ok(result);
    }
}