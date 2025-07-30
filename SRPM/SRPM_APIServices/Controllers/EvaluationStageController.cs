using Microsoft.AspNetCore.Mvc;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.RequestModels.Query;
using SRPM_Services.Interfaces;

namespace SRPM_APIServices.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EvaluationStageController : ControllerBase
{
    //=================================[ Declares ]================================
    private readonly IEvaluationStageService _evaluationStageService;

    public EvaluationStageController(IEvaluationStageService evaluationStageService)
    {
        _evaluationStageService = evaluationStageService;
    }

    //=================================[ Endpoints ]================================
    // api/evaluationstage/123e4567-e89b-12d3-a456-426614174000?incl=1
    [HttpGet("{id}")]
    public async Task<IActionResult> ViewDetail([FromRoute] Guid id, [FromQuery] byte incl)
    {
        var evaluationStage = await _evaluationStageService.ViewDetail(id, incl);
        return Ok(evaluationStage);
    }

    [HttpPost("list")]
    public async Task<IActionResult> List([FromBody] Q_EvaluationStage queryInput)
    {
        var result = await _evaluationStageService.GetListPagingAsync(queryInput);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateEvaluationStage([FromBody] RQ_EvaluationStage newEvaluationStage)
    {
        var result = await _evaluationStageService.CreateAsync(newEvaluationStage);
        return result.success ? Created(nameof(CreateEvaluationStage), result.evaluationStageId)
            : BadRequest("Create Failed!");
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] RQ_EvaluationStage newEvaluationStage)
    {
        bool result = await _evaluationStageService.UpdateAsync(newEvaluationStage);
        return result ? Ok("Update Successfully!") : BadRequest("Update Failed!");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        bool result = await _evaluationStageService.DeleteAsync(id);
        return result ? Ok("Delete Successfully!") : BadRequest("Delete Failed!");
    }
}