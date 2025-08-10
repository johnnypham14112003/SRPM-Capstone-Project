//===================================[ TRACKER ]=================================
/*
 * Drive tasks to other background classes by delegate
 */
//===============================================================================

using SRPM_Services.BusinessModels.Others;
using SRPM_Services.Extensions.Enumerables;
using System.Collections.Concurrent;

namespace SRPM_Services.Extensions.MicrosoftBackgroundService;

public delegate Task BackgroundJob(IServiceProvider serviceProvider, CancellationToken cancellationToken, IProgress<int> progress);

public interface ITaskTracker
{
    public void CreateTask(string taskId, CancellationTokenSource cts);
    public bool TryGetTask(string taskId, out BackgroundTaskInfo info);
    public void SetStatus(string taskId, TrackedTaskStatus status);
    public void UpdateProgress(string taskId, int progress);
    public void SetError(string taskId, string message);
    public void CleanupFinishedTasks();
}

public class TaskTracker : ITaskTracker
{
    private readonly ConcurrentDictionary<string, BackgroundTaskInfo> _tasks = new();

    public void CreateTask(string taskId, CancellationTokenSource cts)
    {
        _tasks[taskId] = new BackgroundTaskInfo
        {
            TaskId = taskId,
            CancellationTokenSource = cts,
            Status = TrackedTaskStatus.Pending,
            Progress = 0
        };
    }

    public bool TryGetTask(string taskId, out BackgroundTaskInfo info) =>
        _tasks.TryGetValue(taskId, out info);

    public void SetStatus(string taskId, TrackedTaskStatus status)
    {
        if (_tasks.TryGetValue(taskId, out var task))
            task.Status = status;
    }

    public void UpdateProgress(string taskId, int progress)
    {
        if (_tasks.TryGetValue(taskId, out var task))
            task.Progress = progress;
    }

    public void SetError(string taskId, string message)
    {
        if (_tasks.TryGetValue(taskId, out var task))
            task.ErrorMessage = message;
    }

    // Clean finished task to prevent memory leak
    public void CleanupFinishedTasks()
    {
        foreach (var key in _tasks.Where(kv =>
            kv.Value.Status is TrackedTaskStatus.Completed
            or TrackedTaskStatus.Cancelled
            or TrackedTaskStatus.Failed)
            .Select(kv => kv.Key))
        {
            _tasks.TryRemove(key, out _);
        }
    }
}