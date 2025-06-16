using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.BusinessModels.ResponseModels
{

    public class RS_IndividualEvaluation
    {
        public Guid Id { get; set; }
        public byte? TotalRate { get; set; }
        public string? Comment { get; set; }
        public DateTime SubmittedAt { get; set; }
        public bool IsApproved { get; set; }
        public bool? ReviewerResult { get; set; }
        public bool IsAIReport { get; set; }
        public string Status { get; set; } = null!;

        public Guid EvaluationStageId { get; set; }
        public Guid? ReviewerId { get; set; }
        public Guid? ProjectId { get; set; }
        public Guid? MilestoneId { get; set; }
    }

}
