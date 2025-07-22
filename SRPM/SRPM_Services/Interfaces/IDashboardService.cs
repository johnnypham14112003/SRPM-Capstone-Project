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
        Task<object> GetUserProjectStatsAsync( DateTime from, DateTime to);
        Task<object> GetPrincipalDashboardAsync( DateTime from, DateTime to);
        Task<object> GetInstitutionDashboardAsync(DateTime from, DateTime to);
        Task<List<object>> GetKpiTilesAsync( DateTime from, DateTime to);
    }

}
