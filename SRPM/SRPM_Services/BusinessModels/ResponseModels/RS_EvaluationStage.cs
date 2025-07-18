using SRPM_Repositories.Models;

namespace SRPM_Services.BusinessModels.ResponseModels;

public class RS_EvaluationStage
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public int StageOrder { get; set; }
    public string Status { get; set; } = null!;
    public Guid EvaluationId { get; set; }
    public Guid? AppraisalCouncilId { get; set; }

    public virtual ICollection<RS_Transaction>? Transactions { get; set; }
    public virtual ICollection<RS_IndividualEvaluation>? IndividualEvaluations { get; set; }
}