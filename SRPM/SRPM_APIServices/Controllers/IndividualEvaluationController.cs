using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.Interfaces;

namespace SRPM_APIServices.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IndividualEvaluationController : ControllerBase
    {
        private readonly IIndividualEvaluationService _service;

        public IndividualEvaluationController(IIndividualEvaluationService service)
        {
            _service = service;
        }

        // GET: api/individualevaluation/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<RS_IndividualEvaluation>> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
                return NotFound($"IndividualEvaluation with ID {id} not found.");
            return Ok(result);
        }

        // GET: api/individualevaluation/by-stage/{evaluationStageId}
        [HttpGet("by-stage/{evaluationStageId}")]
        public async Task<ActionResult<List<RS_IndividualEvaluation>>> GetByStageId(Guid evaluationStageId)
        {
            var results = await _service.GetListByStageAsync(evaluationStageId);
            return Ok(results);
        }

        // POST: api/individualevaluation
        [HttpPost]
        public async Task<ActionResult<RS_IndividualEvaluation>> Create(RQ_IndividualEvaluation request)
        {
            var created = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/individualevaluation/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<RS_IndividualEvaluation>> Update(Guid id, RQ_IndividualEvaluation request)
        {
            var updated = await _service.UpdateAsync(id, request);
            if (updated == null)
                return NotFound($"IndividualEvaluation with ID {id} not found.");
            return Ok(updated);
        }
    }

}
