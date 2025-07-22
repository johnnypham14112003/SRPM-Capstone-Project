using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.BusinessModels.ResponseModels
{

    public class RS_Task
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string? Priority { get; set; }
        public decimal Progress { get; set; }
        public int Overdue { get; set; }
        public string? MeetingUrl { get; set; }
        public string? Note { get; set; }
        public string Status { get; set; } = null!;
        public Guid MilestoneId { get; set; }
        public Guid CreatorId { get; set; }
        public List<RS_MemberTask> MemberTasks { get; set; } = new List<RS_MemberTask>();
    }

}
