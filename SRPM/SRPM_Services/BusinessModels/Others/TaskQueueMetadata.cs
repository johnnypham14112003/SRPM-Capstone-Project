namespace SRPM_Services.BusinessModels.Others;

public class TaskQueueMetadata
{
    public string TaskId { get; set; } = Guid.NewGuid().ToString();
    public TaskStatus Status { get; set; } = TaskStatus.Running;
    public CancellationTokenSource CancellationTokenSource { get; set; } = new();
    public Func<CancellationToken, Task>? TaskFunc { get; set; }
}