using Microsoft.AspNetCore.Mvc;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.Interfaces;

namespace SRPM_APIServices.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EvaluationController : ControllerBase
    {
        private readonly IEvaluationService _evaluationService;

        public EvaluationController(IEvaluationService evaluationService)
        {
            _evaluationService = evaluationService;
        }

        // GET: api/evaluation
        [HttpGet]
        public async Task<ActionResult<List<RS_Evaluation>>> GetAll()
        {
            var evaluations = await _evaluationService.GetListAsync();
            return Ok(evaluations);
        }

        // GET: api/evaluation/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<RS_Evaluation>> GetById(Guid id)
        {
            var evaluation = await _evaluationService.GetByIdAsync(id);
            if (evaluation == null)
                return NotFound($"No evaluation found with ID {id}");

            return Ok(evaluation);
        }

        // POST: api/evaluation
        [HttpPost]
        public async Task<ActionResult<RS_Evaluation>> Create(RQ_Evaluation request)
        {
            var created = await _evaluationService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/evaluation/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<RS_Evaluation>> Update(Guid id, RQ_Evaluation request)
        {
            var updated = await _evaluationService.UpdateAsync(id, request);
            if (updated == null)
                return NotFound($"No evaluation found with ID {id}");

            return Ok(updated);
        }

        // DELETE: api/evaluation/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _evaluationService.DeleteAsync(id);
            if (!success)
                return NotFound($"No evaluation found with ID {id}");

            return NoContent();
        }
    }

}
