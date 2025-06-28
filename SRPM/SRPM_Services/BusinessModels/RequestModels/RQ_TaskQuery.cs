using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.BusinessModels.RequestModels
{
    public class RQ_TaskQuery
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Priority { get; set; }
        public Guid? MilestoneId { get; set; }
        public Guid? CreatorId { get; set; }
        public string? Status { get; set; }

        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

}
