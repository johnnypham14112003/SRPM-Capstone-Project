using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SRPM_Services.Extensions.BackgroundService;

public class BackgroundServiceProvider : BackgroundService
{
    private readonly ITaskQueueHandler _taskQueueHandler;
    private readonly ILogger<BackgroundServiceProvider> _logger;

    public BackgroundServiceProvider(ITaskQueueHandler taskQueueHandler, ILogger<BackgroundServiceProvider> logger)
    {
        _taskQueueHandler = taskQueueHandler;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var meta = await _taskQueueHandler.DequeueTrackedAsync(stoppingToken);
        }
    }
}