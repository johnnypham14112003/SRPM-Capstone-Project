using Microsoft.AspNetCore.Mvc;
using SRPM_Services.BusinessModels;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.RequestModels.Query;
using SRPM_Services.BusinessModels.ResponseModels;
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

    [HttpPost("list-project")]
    public async Task<IActionResult> GetProjectOfCouncil([FromBody] ListProjectOfCouncil query)
    {
        if (query.isProposal == true)
        {
            var proposals = await _appraisalCouncilService
                .GetProposalsFromCouncilAsync(query.councilId);

            if (proposals == null || !proposals.Any())
                return NotFound("No proposal projects found for this CouncilId");

            if (query.statuses is not null && query.statuses.Any())
            {
                var filtered = proposals
                    .Where(p => query.statuses
                        .Contains(p.Status.ToString().ToLower()))
                    .ToList();

                if (!filtered.Any())
                    return NotFound("No proposals match the given statuses");

                return Ok(filtered);
            }

            return Ok(proposals);
        }

        var councilProjects = await _appraisalCouncilService
            .GetProjectsFromCouncilAsync(query.councilId);

        if (councilProjects == null || !councilProjects.Any())
            return NotFound("No projects found for this CouncilId");

        if (query.statuses is not null && query.statuses.Any())
        {
            var filteredByStatus = councilProjects
                .Where(p => query.statuses
                    .Contains(p.Status.ToString().ToLower()))
                .ToList();

            if (!filteredByStatus.Any())
                return NotFound("No projects match the given statuses");

            return Ok(filteredByStatus);
        }

        return Ok(councilProjects);
    }         
    public sealed record ListProjectOfCouncil()
    {
        public Guid councilId { get; set; }
        public List<string>? statuses { get; set; }
        public bool? isProposal { get; set; }
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
    [HttpGet("online-user")]
    public async Task<ActionResult<PagingResult<RS_AppraisalCouncil>>> GetOnlineUserCouncils(
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10)
    {
        if (pageIndex <= 0 || pageSize <= 0)
            return BadRequest("PageIndex and PageSize must be greater than zero.");

        var result = await _appraisalCouncilService.GetAllOnlineUserAppraisalCouncilAsync(pageIndex, pageSize);

        return Ok(result);
    }


}