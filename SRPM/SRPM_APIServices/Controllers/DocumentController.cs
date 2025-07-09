using Microsoft.AspNetCore.Mvc;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.RequestModels.Query;
using SRPM_Services.Interfaces;

namespace SRPM_APIServices.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentController : Controller
{
    //=================================[ Declares ]================================
    private readonly IDocumentService _documentService;

    public DocumentController(IDocumentService documentService)
    {
        _documentService = documentService;
    }

    //=================================[ Endpoints ]================================
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] RQ_Document inputData)
    {
        var result = await _documentService.NewDocument(inputData);
        return result.result ? Created(nameof(Add), "Create Successfully!") : BadRequest("Create Failed!");
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ViewDetail([FromRoute] Guid id)
    {
        var result = await _documentService.ViewDetailDocument(id);
        return Ok(result);
    }

    [HttpPost("list")]
    public async Task<IActionResult> List([FromBody] Q_Document queryInput)
    {
        var result = await _documentService.ListDocument(queryInput);
        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] RQ_Document inputData)
    {
        bool result = await _documentService.UpdateDocumentInfo(inputData);
        return result ? Ok("Update Successfully!") : BadRequest("Update Failed!");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        bool result = await _documentService.DeleteDocument(id);
        return result ? Ok("Delete Successfully!") : BadRequest("Delete Failed!");
    }
}