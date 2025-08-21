using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.Interfaces
{
    public interface IDashboardService
    {
        Task<object> GetSystemStatsAsync(DateTime from, DateTime to);
        Task<object> GetTransactionStatsAsync(DateTime from, DateTime to);
        Task<object> GetMilestoneProgressStatsAsync();
        Task<object> GetCouncilProjectStatsAsync();
        Task<object> GetSourceProjectStatusStatsAsync(DateTime from, DateTime to);
        Task<List<object>> GetBaseUserRoleDistributionAsync();
        Task<Dictionary<string, object>> GetTimeSeriesStatsAsync(DateTime? from, DateTime to, string granularity = "daily");
        Task<List<object>> GetMajorDistributionStatsAsync(DateTime from, DateTime to);
    }

}
