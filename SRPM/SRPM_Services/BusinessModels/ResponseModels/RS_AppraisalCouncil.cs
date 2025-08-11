using SRPM_Repositories.Models;

namespace SRPM_Services.BusinessModels.ResponseModels;

public class RS_AppraisalCouncil
{
    public Guid? Id { get; set; }
    public string Code { get; set; } = null!;
    public string? Name { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string Status { get; set; } = "created";

    public virtual ICollection<RS_Evaluation>? Evaluations { get; set; }
    public virtual ICollection<RS_EvaluationStage>? EvaluationStages { get; set; }
    public virtual ICollection<RS_UserRole>? CouncilMembers { get; set; }
}