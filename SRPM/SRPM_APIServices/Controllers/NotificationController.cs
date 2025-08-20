using Microsoft.AspNetCore.Mvc;
using SRPM_Services.BusinessModels.Others;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.RequestModels.Query;
using SRPM_Services.Interfaces;

namespace SRPM_APIServices.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : Controller
{
    //=================================[ Declares ]================================
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    //=================================[ Endpoints ]================================
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] RQ_Notification inputData)
    {
        var result = await _notificationService.CreateNew(inputData);
        return result.Item1 ? Created(nameof(Add), result.Item2) : BadRequest("Create Failed!");
    }

    [HttpPost("list")]
    public async Task<IActionResult> ListNotfication([FromBody] Q_RequestNoti queryInput)
    {
        var result = await _notificationService.ListRequestNotification(queryInput);
        return Ok(result);
    }

    [HttpPost("user-notification")]
    public async Task<IActionResult> ListNotiOfUser([FromBody] Q_AccountNotification queryInput)
    {
        var result = await _notificationService.ListNotificationOfUser(queryInput);
        return Ok(result);
    }

    [HttpPost("accounts")]
    public async Task<IActionResult> NotiToUser([FromBody] RQ_NotificationToUsers input)
    {
        var result = await _notificationService.NotificateToUser(input.ListAccountId, input.NotificationId);
        return Ok(result);
    }

    [HttpPost("email")]
    public async Task<IActionResult> SendEmail([FromBody] RQ_NotificationEmail notification)
    {
        bool result = await _notificationService.SendNotificationMail(notification);
        return result ? Ok("Update Successfully!") : BadRequest("Update Failed!");
    }

    [HttpPut("status")]
    public async Task<IActionResult> CheckReadNoti([FromQuery] Guid? notification)
    {
        bool result = await _notificationService.ReadNotification(notification);
        return result ? Ok("Update Successfully!") : BadRequest("Update Failed!");
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] RQ_Notification inputData)
    {
        bool result = await _notificationService.UpdateNotification(inputData);
        return result ? Ok("Update Successfully!") : BadRequest("Update Failed!");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        bool result = await _notificationService.DeleteNotification(id);
        return result ? Ok("Delete Successfully!") : BadRequest("Delete Failed!");
    }
}