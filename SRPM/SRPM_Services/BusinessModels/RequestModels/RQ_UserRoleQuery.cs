using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.BusinessModels.RequestModels
{
    public class RQ_UserRoleQuery
    {
        public Guid? AccountId { get; set; }
        public Guid? RoleId { get; set; }
        public Guid? ProjectId { get; set; }
        public Guid? AppraisalCouncilId { get; set; }
        public string? Status { get; set; }
        public bool? IsOfficial { get; set; }

        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

}
