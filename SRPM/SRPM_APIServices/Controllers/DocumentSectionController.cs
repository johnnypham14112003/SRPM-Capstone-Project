using Microsoft.AspNetCore.Mvc;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.RequestModels.Query;
using SRPM_Services.Implements;
using SRPM_Services.Interfaces;

namespace SRPM_APIServices.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentSectionController : Controller
{
    //=================================[ Declares ]================================
    private readonly IDocumentSectionService _documentSectionService;

    public DocumentSectionController(IDocumentSectionService documentSectionService)
    {
        _documentSectionService = documentSectionService;
    }

    //=================================[ Endpoints ]================================
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] RQ_DocumentSection inputData)
    {
        var result = await _documentSectionService.AddSection(inputData);
        return result.result ? Created(nameof(Add), "Add Successfully!") : BadRequest("Add Failed!");
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] RQ_DocumentSection inputData)
    {
        bool result = await _documentSectionService.UpdateDocumentSection(inputData);
        return result ? Ok("Update Successfully!") : BadRequest("Update Failed!");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        bool result = await _documentSectionService.DeleteDocumentSection(id);
        return result ? Ok("Delete Successfully!") : BadRequest("Delete Failed!");
    }
}
