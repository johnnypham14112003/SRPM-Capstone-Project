using SRPM_Services.Extensions.Enumerables;

namespace SRPM_Services.BusinessModels.Others;

public class BackgroundTaskInfo
{
    public string TaskId { get; set; }
    public TrackedTaskStatus Status { get; set; } = TrackedTaskStatus.Pending;
    public int Progress { get; set; } = 0;
    public CancellationTokenSource CancellationTokenSource { get; set; }
    public string ErrorMessage { get; set; }
}