namespace SRPM_Services.Extensions.Enumerables
{
    public enum Status
    {
        Draft,
        Created,
        Submitted,
        Approved,
        InProgress,
        Pending,
        Completed,
        Rejected,
        Deleted
    }
    public static class StatusExtensions
    {
        public static Status ToStatus(this string value)
        {
            if (Enum.TryParse<Status>(value, true, out var parsed))
                return parsed;

            var validValues = string.Join(", ", Enum.GetNames(typeof(Status)));
            throw new ArgumentException($"Invalid status value: '{value}'. Valid statuses are: {validValues}.");
        }
    }
    public enum TaskStatus
    {
        ToDo,
        InProgress,
        Completed,
        Overdue
    }
    public static class TaskStatusExtensions
    {
        public static TaskStatus ToTaskStatus(this string value)
        {
            if (Enum.TryParse<TaskStatus>(value.Trim(), true, out var parsed))
                return parsed;

            var validValues = string.Join(", ", Enum.GetNames(typeof(TaskStatus)));
            throw new ArgumentException($"Invalid status value: '{value}'. Valid statuses are: {validValues}.");
        }
        public static string ToFriendlyString(this TaskStatus status)
        {
            return status switch
            {
                TaskStatus.ToDo => "To Do",
                TaskStatus.InProgress => "In Progress",
                TaskStatus.Completed => "Completed",
                TaskStatus.Overdue => "Overdue",
                _ => status.ToString()
            };
        }
    }
}
