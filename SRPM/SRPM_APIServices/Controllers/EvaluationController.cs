using Microsoft.AspNetCore.Mvc;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.RequestModels.Query;
using SRPM_Services.BusinessModels.ResponseModels;
using SRPM_Services.Interfaces;

namespace SRPM_APIServices.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EvaluationController : Controller
{
    //=================================[ Declares ]================================
    private readonly IEvaluationService _evaluationService;

    public EvaluationController(IEvaluationService evaluationService)
    {
        _evaluationService = evaluationService;
    }

    //=================================[ Endpoints ]================================
    [HttpPost("list")]
    public async Task<IActionResult> List([FromBody] Q_Evaluation queryInput)
    {
        var result = await _evaluationService.GetListAsync(queryInput);
        return Ok(result);
    }

    // api/evaluation/123e4567-e89b-12d3-a456-426614174000?incl=1
    [HttpGet("{id}")]
    public async Task<IActionResult> ViewDetailEvaluation([FromRoute] Guid id, [FromQuery] byte incl)
    {
        var evaluation = await _evaluationService.ViewDetail(id, incl);
        return Ok(evaluation);
    }

    [HttpPost]
    public async Task<IActionResult> CreateEvaluation([FromBody] RQ_Evaluation newEvaluation)
    {
        var result = await _evaluationService.CreateAsync(newEvaluation);
        return result.success ? Created(nameof(CreateEvaluation), result.evaluationId)
            : BadRequest("Create Failed!");
    }

    [HttpPost("first-evaluation")]
    public async Task<IActionResult> AICreateEvaluation([FromQuery] Guid projectId)
    {
        string bgTaskId = await _evaluationService.FirstAIEvaluation(projectId);
        return Ok(bgTaskId);
    }

    [HttpPost("project-similarity")]
    public async Task<IActionResult> AIRegenEvaluation([FromBody] RQ_PlagiarismTarget target)
    {
        string bgTaskId = await _evaluationService.RegenAIEvaluation(target.ProjectId, target.individualEvalutionId);
        return Ok(bgTaskId);
    }

    [HttpPut]
    public async Task<ActionResult<RS_Evaluation>> Update([FromBody] RQ_Evaluation newEvaluation)
    {
        bool result = await _evaluationService.UpdateAsync(newEvaluation);
        return result ? Ok("Update Successfully!") : BadRequest("Update Failed!");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        bool result = await _evaluationService.DeleteAsync(id);
        return result ? Ok("Delete Successfully!") : BadRequest("Delete Failed!");
    }
}