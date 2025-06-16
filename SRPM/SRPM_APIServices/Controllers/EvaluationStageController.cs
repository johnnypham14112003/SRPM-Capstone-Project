using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.Interfaces;

namespace SRPM_APIServices.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EvaluationStageController : ControllerBase
    {
        private readonly IEvaluationStageService _service;

        public EvaluationStageController(IEvaluationStageService service)
        {
            _service = service;
        }

        // GET: api/evaluationstage/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<RS_EvaluationStage>> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
                return NotFound($"EvaluationStage with ID {id} not found.");

            return Ok(result);
        }

        // GET: api/evaluationstage/by-evaluation/{evaluationId}
        [HttpGet("by-evaluation/{evaluationId}")]
        public async Task<ActionResult<List<RS_EvaluationStage>>> GetByEvaluationId(Guid evaluationId)
        {
            var results = await _service.GetListByEvaluationIdAsync(evaluationId);
            return Ok(results);
        }

        // POST: api/evaluationstage
        [HttpPost]
        public async Task<ActionResult<RS_EvaluationStage>> Create(RQ_EvaluationStage request)
        {
            var created = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/evaluationstage/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<RS_EvaluationStage>> Update(Guid id, RQ_EvaluationStage request)
        {
            var updated = await _service.UpdateAsync(id, request);
            if (updated == null)
                return NotFound($"EvaluationStage with ID {id} not found.");

            return Ok(updated);
        }

        // DELETE: api/evaluationstage/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success)
                return NotFound($"EvaluationStage with ID {id} not found.");

            return NoContent();
        }
    }

}
