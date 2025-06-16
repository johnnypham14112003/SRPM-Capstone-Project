using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.BusinessModels.RequestModels
{
    public class RQ_Evaluation
    {
        public string Title { get; set; } = null!;
        public string? Comment { get; set; }
        public byte? TotalRate { get; set; }
        public string Phrase { get; set; } = "proposal";
        public string Type { get; set; } = "project"; // or "milestone"
        public Guid ProjectId { get; set; }
        public Guid? MilestoneId { get; set; }
        public Guid? AppraisalCouncilId { get; set; }
    }

}
