using Microsoft.AspNetCore.Mvc;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.RequestModels.Query;
using SRPM_Services.Extensions.Exceptions;
using SRPM_Services.Implements;
using SRPM_Services.Interfaces;

namespace SRPM_APIServices.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppraisalCouncilController : Controller
{
    //=================================[ Declares ]================================
    private readonly IAppraisalCouncilService _appraisalCouncilService;

    public AppraisalCouncilController(IAppraisalCouncilService appraisalCouncilService)
    {
        _appraisalCouncilService = appraisalCouncilService;
    }

    //=================================[ Endpoints ]================================
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] RQ_AppraisalCouncil inputData)
    {
        var result = await _appraisalCouncilService.NewAppraisal(inputData);
        return result.result ? Created(nameof(Add), result.CoucilId) : BadRequest("Create Failed!");
    }

    // api/appraisalcouncil/123e4567-e89b-12d3-a456-426614174000?incl=1
    [HttpGet("{id}")]
    public async Task<IActionResult> ViewDetail([FromRoute] Guid id, [FromQuery] byte incl)
    {
        //IncludeNo 1:Evaluations | 2:EvaluationStages | 3:Members | 12, 13, 23...
        var result = await _appraisalCouncilService.ViewDetailCouncil(id, incl);
        return Ok(result);
    }

    [HttpPost("list")]
    public async Task<IActionResult> List([FromBody] Q_AppraisalCouncil queryInput)
    {
        var result = await _appraisalCouncilService.ListCouncil(queryInput);
        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] RQ_AppraisalCouncil inputData)
    {
        bool result = await _appraisalCouncilService.UpdateCouncilInfo(inputData);
        return result ? Ok("Update Successfully!") : BadRequest("Update Failed!");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        bool result = await _appraisalCouncilService.DeleteCouncil(id);
        return result ? Ok("Delete Successfully!") : BadRequest("Delete Failed!");
    }

    [HttpGet("list-project/{councilId}")]
    public async Task<IActionResult> GetProjectOfCouncil([FromRoute] Guid councilId)
    {
        var result = await _appraisalCouncilService.GetProjectsFromCouncilAsync(councilId);

        return result is not null ? Ok(result) : NotFound("Not found any Project belong to this CouncilId");
    }

    [HttpGet("project/{projectId}")]
    public async Task<IActionResult> GetCouncilOfProject([FromRoute] Guid projectId)
    {
        var council = await _appraisalCouncilService.GetCouncilInEvaluationAsync(projectId);

        return council is not null? Ok(council) : NotFound("Not found any Appraisal Council belong to this ProjectId");
    }

    [HttpPost("assign-council")]
    public async Task<IActionResult> AssignCouncilToClonedStages([FromQuery] Guid sourceProjectId, Guid appraisalCouncilId)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = await _appraisalCouncilService.AssignCouncilToClonedStages(
                sourceProjectId,
                appraisalCouncilId
            );

            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Unexpected error: {ex.Message}");
        }
    }

}