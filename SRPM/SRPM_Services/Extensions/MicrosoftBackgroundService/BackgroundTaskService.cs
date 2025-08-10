//===================================[ SERVICE ]===================================
/*
 * Do the job in queue
 * Background worker runs continuously in the background while ASP.NET starts.
 * Continuously call DequeueAsync to get new jobs.
 */
//=================================================================================

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SRPM_Services.Extensions.Enumerables;

namespace SRPM_Services.Extensions.MicrosoftBackgroundService;

public class BackgroundTaskService : BackgroundService
{
    private readonly IBackgroundTaskQueue _backgroundTaskQueue;
    private readonly ITaskTracker _tracker;
    private readonly ILogger<BackgroundTaskService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public BackgroundTaskService(
        IBackgroundTaskQueue backgroundTaskQueue,
        ITaskTracker tracker,
        ILogger<BackgroundTaskService> logger,
        IServiceScopeFactory scopeFactory)
    {
        _backgroundTaskQueue = backgroundTaskQueue;
        _tracker = tracker;
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Background task processing service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var (taskId, workItem) = await _backgroundTaskQueue.DequeueAsync(stoppingToken);

                if (!_tracker.TryGetTask(taskId, out var info))
                    continue;

                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken, info.CancellationTokenSource.Token);
                var linkedToken = linkedCts.Token;
                var progress = new Progress<int>(p => _tracker.UpdateProgress(taskId, p));

                _tracker.SetStatus(taskId, TrackedTaskStatus.Running);

                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var serviceProvider = scope.ServiceProvider;

                    await workItem(serviceProvider, linkedToken, progress);

                    if (linkedToken.IsCancellationRequested && info.CancellationTokenSource.IsCancellationRequested)
                        _tracker.SetStatus(taskId, TrackedTaskStatus.Cancelled);
                    else
                    {
                        _tracker.UpdateProgress(taskId, 100);
                        _tracker.SetStatus(taskId, TrackedTaskStatus.Completed);
                    }
                }
                catch (OperationCanceledException)
                {
                    _tracker.SetStatus(taskId, TrackedTaskStatus.Cancelled);
                }
                catch (Exception ex)
                {
                    _tracker.SetError(taskId, ex.Message);
                    _tracker.SetStatus(taskId, TrackedTaskStatus.Failed);
                }
                finally
                {
                    _tracker.CleanupFinishedTasks();
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in task processing loop.");
                await Task.Delay(1000, stoppingToken);
            }
        }

        _logger.LogInformation("Background task processing service stopping.");
    }
}