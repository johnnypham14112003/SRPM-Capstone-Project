

namespace SRPM_Services.BusinessModels.RequestModels
{
    public class RQ_EvaluationStage
    {
        public string? Name { get; set; }
        public int StageOrder { get; set; } = 1;
        public string? Status { get; set; } // optional override
        public Guid EvaluationId { get; set; }
        public Guid? AppraisalCouncilId { get; set; }
    }

}
