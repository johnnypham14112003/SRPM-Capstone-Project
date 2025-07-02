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

    public virtual ICollection<Evaluation>? Evaluations { get; set; }
    public virtual ICollection<EvaluationStage>? EvaluationStages { get; set; }
    public virtual ICollection<UserRole>? CouncilMembers { get; set; }
}