using Microsoft.AspNetCore.Mvc;
using SRPM_Services.Extensions.BackgroundService;

namespace SRPM_APIServices.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BackgroundTaskController : Controller
{
    private readonly ITaskQueueHandler _taskQueueHandler;

    public BackgroundTaskController(ITaskQueueHandler taskQueueHandler)
    {
        _taskQueueHandler = taskQueueHandler;
    }

    [HttpGet("{taskId}")]
    public IActionResult GetStatus([FromRoute] string taskId)
    {
        var meta = _taskQueueHandler.GetTaskMetadata(taskId);
        if (meta == null) return NotFound();
        return Ok(new
        {
            TaskId = meta.TaskId,
            Status = meta.Status.ToString()
        });
    }

    [HttpDelete("{taskId}")]
    public IActionResult Cancel([FromRoute] string taskId)
    {
        var success = _taskQueueHandler.CancelTask(taskId);
        if (!success) return NotFound("Cannot Cancel Or It Has Been Completed!");
        return Ok("Cancelled Task");
    }
}