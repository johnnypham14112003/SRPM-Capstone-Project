using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.BusinessModels.RequestModels
{
    public class RQ_ProposalSubmission
    {
        public List<RQ_ProjectMember> Members { get; set; } = new();
        public List<RQ_Document> Documents { get; set; } = new();
        public string Note { get; set; } = string.Empty;
    }
    public class RQ_ProjectMember
    {
        public Guid AccountId { get; set; }
        public Guid RoleId { get; set; } 
    }


}
