using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.BusinessModels.RequestModels
{
    public class RQ_UserRole
    {
        public string? GroupName { get; set; }
        public bool IsOfficial { get; set; }
        public DateTime? ExpireDate { get; set; }
        public Guid AccountId { get; set; }
        public Guid RoleId { get; set; }
        public Guid? ProjectId { get; set; }
        public Guid? AppraisalCouncilId { get; set; }
    }


}
