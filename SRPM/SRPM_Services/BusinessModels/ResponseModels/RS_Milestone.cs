using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.BusinessModels.ResponseModels
{
    public class RS_Milestone
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string? Objective { get; set; }
        public decimal Cost { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Type { get; set; } = "normal";
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = null!;

        public Guid ProjectId { get; set; }
        public Guid CreatorId { get; set; }

        // 🌐 Related Models

        public RS_Project? Project { get; set; }
        public RS_UserRole? Creator { get; set; }
        public List<RS_Evaluation>? Evaluations { get; set; }
        public List<RS_Task>? Tasks { get; set; }
    }



}
