//===================================[ QUEUE ]===================================
/*
 * A queue storage for class services call -> push delegate 
 * Contains ConcurrentDictionary or ConcurrentQueue to store taskId + delegate to handle.
 */
//===============================================================================
using System.Collections.Concurrent;

namespace SRPM_Services.Extensions.MicrosoftBackgroundService;

// Interface for queue
public interface IBackgroundTaskQueue
{
    void QueueBackgroundWorkItem(string taskId, BackgroundJob workItem);
    Task<(string taskId, BackgroundJob workItem)> DequeueAsync(CancellationToken cancellationToken);
}

public class BackgroundTaskQueue : IBackgroundTaskQueue
{
    private readonly ConcurrentQueue<(string TaskId, BackgroundJob WorkItem)> _workItems = new();
    private readonly SemaphoreSlim _signal = new(0);

    public void QueueBackgroundWorkItem(string taskId, BackgroundJob workItem)
    {
        _workItems.Enqueue((taskId, workItem));
        _signal.Release();
    }

    public async Task<(string taskId, BackgroundJob workItem)> DequeueAsync(CancellationToken cancellationToken)
    {
        await _signal.WaitAsync(cancellationToken);
        _workItems.TryDequeue(out var workItem);
        return workItem;
    }
}