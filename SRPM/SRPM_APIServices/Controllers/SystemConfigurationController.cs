using Microsoft.AspNetCore.Mvc;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.Interfaces;

namespace SRPM_APIServices.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SystemConfigurationController : Controller
{
    //=================================[ Declares ]================================
    private readonly ISystemConfigurationService _systemConfigurationService;

    public SystemConfigurationController(ISystemConfigurationService systemConfigurationService)
    {
        _systemConfigurationService = systemConfigurationService;
    }

    //=================================[ Endpoints ]================================
    [HttpPost]
    public async Task<IActionResult> AddNew([FromBody] RQ_SystemConfiguration inputData)
    {
        var response = await _systemConfigurationService.AddNewConfig(inputData);

        if (response.scResult == false) return BadRequest("Create Failed!");

        if (response.scResult == true && response.notiResult == false)
            return Created(nameof(AddNew), "Create System Configuration Successfully but failed to create Notification!");

        return Created(nameof(AddNew), "Create Successfully!");
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ViewDetail([FromRoute] Guid id)
    {
        var configyInfo = await _systemConfigurationService.ViewDetailConfig(id);
        return Ok(configyInfo);
    }

    // api/systemconfiguration?typeData...&keyData=...
    [HttpGet]
    public async Task<IActionResult> ListConfig([FromQuery] string typeData, [FromQuery] string? keyData)
    {
        var categoryInfo = await _systemConfigurationService.ListConfig(typeData, keyData);
        return Ok(categoryInfo);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateConfig([FromBody] RQ_SystemConfiguration inputData)
    {
        bool result = await _systemConfigurationService.ChangeConfig(inputData);
        return result ? Ok("Update Successfully!") : BadRequest("Update Failed!");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory([FromRoute] Guid id)
    {
        bool result = await _systemConfigurationService.RemoveConfig(id);
        return result ? Ok("Delete Successfully!") : BadRequest("Delete Failed!");
    }
}