//===============================[ HANDLER ]===============================
/*
 * Get item from Queue by its delegate
 * Listen for tasks pushed to BackgroundTaskQueue
 * Tasks are executed in FIFO order.
 */
//=========================================================================

namespace SRPM_Services.Extensions.MicrosoftBackgroundService;

public interface ITaskQueueHandler
{
    public string EnqueueTracked(BackgroundJob workItem);
}

public class TaskQueueHandler : ITaskQueueHandler
{
    private readonly IBackgroundTaskQueue _queue;
    private readonly ITaskTracker _tracker;

    public TaskQueueHandler(IBackgroundTaskQueue queue, ITaskTracker tracker)
    {
        _queue = queue;
        _tracker = tracker;
    }

    public string EnqueueTracked(BackgroundJob workItem)
    {
        var taskId = Guid.NewGuid().ToString();
        var cts = new CancellationTokenSource();
        _tracker.CreateTask(taskId, cts);
        _queue.QueueBackgroundWorkItem(taskId, workItem);
        return taskId;
    }
}