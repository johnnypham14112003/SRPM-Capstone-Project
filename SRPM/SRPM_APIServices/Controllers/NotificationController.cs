using Microsoft.AspNetCore.Mvc;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.Interfaces;

namespace SRPM_APIServices.Controllers
{
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
            return result.Item1 ? Created(nameof(Add), "Create Successfully!") : BadRequest("Create Failed!");
        }

        [HttpGet("list")]
        public async Task<IActionResult> ListOfUser([FromBody] RQ_QueryAccountNotification queryInput)
        {
            var result = await _notificationService.ListNotificationOfUser(queryInput);
            return Ok(result);
        }

        [HttpPatch]
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
}
