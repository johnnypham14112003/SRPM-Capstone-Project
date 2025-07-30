using Microsoft.AspNetCore.Mvc;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.RequestModels.Query;
using SRPM_Services.Interfaces;

namespace SRPM_APIServices.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IndividualEvaluationController : ControllerBase
{
    private readonly IIndividualEvaluationService _individualEvaluationService;

    public IndividualEvaluationController(IIndividualEvaluationService individualEvaluationService)
    {
        _individualEvaluationService = individualEvaluationService;
    }

    //=================================[ Endpoints ]================================
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] RQ_IndividualEvaluation inputData)
    {
        var result = await _individualEvaluationService.CreateAsync(inputData);
        return result.success ? Created(nameof(Add), result.individualEvaluationId)
            : BadRequest("Create Failed!");
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ViewDetail([FromRoute] Guid id)
    {
        var result = await _individualEvaluationService.ViewDetail(id);
        return Ok(result);
    }

    [HttpPost("list")]
    public async Task<IActionResult> List([FromBody] Q_IndividualEvaluation queryInput)
    {
        var result = await _individualEvaluationService.GetListAsync(queryInput);
        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] RQ_IndividualEvaluation inputData)
    {
        bool result = await _individualEvaluationService.UpdateAsync(inputData);
        return result ? Ok("Update Successfully!") : BadRequest("Update Failed!");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        bool result = await _individualEvaluationService.DeleteAsync(id);
        return result ? Ok("Delete Successfully!") : BadRequest("Delete Failed!");
    }
}