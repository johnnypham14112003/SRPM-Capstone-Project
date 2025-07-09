using System.Threading.Channels;

namespace SRPM_Services.Extensions.BackgroundService;

public interface ITaskQueueHandler
{
    void Enqueue(Func<CancellationToken, Task> task);
    Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
}

public class TaskQueueHandler : ITaskQueueHandler
{
    private readonly Channel<Func<CancellationToken, Task>> _queue = Channel.CreateUnbounded<Func<CancellationToken, Task>>();

    public void Enqueue(Func<CancellationToken, Task> task) =>
        _queue.Writer.TryWrite(task);

    public async Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken) =>
        await _queue.Reader.ReadAsync(cancellationToken);
}