using Microsoft.AspNetCore.Mvc;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.RequestModels.Query;
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
        return result.result ? Created(nameof(Add), "Create Successfully!") : BadRequest("Create Failed!");
    }

    [HttpGet]
    public async Task<IActionResult> ViewDetail([FromBody] RQ_AppraisalCouncil council)
    {
        var result = await _appraisalCouncilService.ViewDetailCouncil(council.Id, council.IncludeNo);
        return Ok(result);
    }

    [HttpGet("list")]
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
}