using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.BusinessModels.ResponseModels
{
    public class RS_UserRole
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = null!;
        public string? GroupName { get; set; }
        public bool IsOfficial { get; set; }
        public DateTime? ExpireDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = "created";
        public Guid AccountId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? AvatarURL { get; set; }
        public Guid RoleId { get; set; }
        public string Name { get; set; } = null!;
        public Guid? ProjectId { get; set; }
        public Guid? AppraisalCouncilId { get; set; }
    }

}
