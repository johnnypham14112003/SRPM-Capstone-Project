using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.BusinessModels.ResponseModels
{
    public class RS_MemberTask
    {
        public Guid Id { get; set; }
        public decimal Progress { get; set; }
        public int Overdue { get; set; }
        public string? Note { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime JoinedAt { get; set; }
        public string Status { get; set; } = null!;
        public Guid MemberId { get; set; }
        public string FullName { get; set; } = null!;
        public string? AvatarUrl { get; set; }
        public string RoleName { get; set; } = null!;

        public Guid TaskId { get; set; }
    }


}
