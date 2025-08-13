using Microsoft.AspNetCore.Mvc;
using SRPM_Services.Extensions.Enumerables;
using SRPM_Services.Extensions.MicrosoftBackgroundService;

namespace SRPM_APIServices.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BackgroundTaskController : Controller
{
    private readonly ITaskTracker _tracker;

    public BackgroundTaskController(ITaskTracker tracker)
    {
        _tracker = tracker;
    }

    [HttpGet("{taskId}")]
    public IActionResult GetTaskStatus(string taskId)
    {
        if (_tracker.TryGetTask(taskId, out var info))
        {
            return Ok(new
            {
                info.TaskId,
                info.Status,
                info.Progress,
                info.ErrorMessage
            });
        }

        return NotFound(new { message = "Task not found or already cleaned up." });
    }

    [HttpDelete("{taskId}")]
    public IActionResult CancelTask(string taskId)
    {
        if (_tracker.TryGetTask(taskId, out var info))
        {
            info.CancellationTokenSource.Cancel();
            return Ok(new { message = "Task cancelled" });
        }

        return NotFound(new { message = "Task not found" });
    }
}