using SRPM_Services.BusinessModels.Others;
using System.Collections.Concurrent;
using System.Threading.Channels;

namespace SRPM_Services.Extensions.BackgroundService;

public interface ITaskQueueHandler
{
    string EnqueueTracked(Func<CancellationToken, Task> task);
    Task<TaskQueueMetadata?> DequeueTrackedAsync(CancellationToken cancellationToken);
    TaskQueueMetadata? GetTaskMetadata(string taskId);
    bool CancelTask(string taskId);
}

public class TaskQueueHandler : ITaskQueueHandler
{
    private readonly Channel<TaskQueueMetadata> _queue = Channel.CreateUnbounded<TaskQueueMetadata>();
    private readonly ConcurrentDictionary<string, TaskQueueMetadata> _taskRegistry = new();

    public string EnqueueTracked(Func<CancellationToken, Task> task)
    {
        var meta = new TaskQueueMetadata
        {
            TaskFunc = task
        };
        _taskRegistry[meta.TaskId] = meta;
        _queue.Writer.TryWrite(meta);
        return meta.TaskId;
    }

    public async Task<TaskQueueMetadata?> DequeueTrackedAsync(CancellationToken cancellationToken)
    {
        var meta = await _queue.Reader.ReadAsync(cancellationToken);
        if (meta != null)
        {
            meta.Status = TaskStatus.Running;

            _ = Task.Run(async () =>
            {
                try
                {
                    await meta.TaskFunc!(meta.CancellationTokenSource.Token);
                    meta.Status = TaskStatus.RanToCompletion;
                }
                catch (OperationCanceledException)
                {
                    meta.Status = TaskStatus.Canceled;
                }
                catch
                {
                    meta.Status = TaskStatus.Faulted;
                }
            });

            return meta;
        }

        return null;
    }

    public TaskQueueMetadata? GetTaskMetadata(string taskId) =>
        _taskRegistry.TryGetValue(taskId, out var meta) ? meta : null;

    public bool CancelTask(string taskId)
    {
        if (_taskRegistry.TryGetValue(taskId, out var meta))
        {
            meta.CancellationTokenSource.Cancel();
            return true;
        }
        return false;
    }
}