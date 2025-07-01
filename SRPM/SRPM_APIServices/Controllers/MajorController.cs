using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SRPM_Services.BusinessModels;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.Interfaces;

namespace SRPM_APIServices.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MajorController : ControllerBase
    {
        private readonly IMajorService _service;

        public MajorController(IMajorService service)
        {
            _service = service;
        }

        // GET: api/major/filter
        [HttpGet("filter")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<PagingResult<RS_Major>>> GetList([FromQuery] RQ_MajorQuery query)
        {
            var result = await _service.GetListAsync(query);
            return Ok(result);
        }


        // GET: api/major/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<RS_Major>> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
                return NotFound($"Major with ID {id} not found.");
            return Ok(result);
        }

        // POST: api/major
        [HttpPost]
        public async Task<ActionResult<RS_Major>> Create(RQ_Major request)
        {
            var created = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/major/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<RS_Major>> Update(Guid id, RQ_Major request)
        {
            var updated = await _service.UpdateAsync(id, request);
            if (updated == null)
                return NotFound($"Major with ID {id} not found.");
            return Ok(updated);
        }

        // DELETE: api/major/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
                return NotFound($"Major with ID {id} not found.");
            return NoContent();
        }
    }

}
