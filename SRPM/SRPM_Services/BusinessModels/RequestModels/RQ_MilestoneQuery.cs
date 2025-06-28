using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.BusinessModels.RequestModels
{
    public class RQ_MilestoneQuery
    {
        public string? Title { get; set; }
        public string? Code { get; set; }
        public string? Type { get; set; }
        public string? Status { get; set; }
        public Guid? ProjectId { get; set; }
        public Guid? CreatorId { get; set; }

        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

}
