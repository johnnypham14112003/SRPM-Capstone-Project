using Newtonsoft.Json.Linq;
using System;

namespace SRPM_Services.Extensions.Enumerables
{
    public enum Status
    {
        Draft,
        Created,
        Submitted,
        Approved,
        InProgress,
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
}

