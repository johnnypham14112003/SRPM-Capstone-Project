using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models;
public class EvaluationStage
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();

    [MaxLength(255)] public string? Name { get; set; }
    [Required] public int StageOrder { get; set; } = 1;
    [Required, MaxLength(30)] public string Status { get; set; } = "created";

    // Foreign keys
    [Required] public Guid EvaluationId { get; set; }
    public Guid? AppraisalCouncilId { get; set; }

    // Navigation properties
    public virtual Evaluation Evaluation { get; set; } = null!;
    public virtual AppraisalCouncil? AppraisalCouncil { get; set; }//one stage - one council
    public virtual ICollection<Transaction>? Transactions { get; set; }
    public virtual ICollection<IndividualEvaluation>? IndividualEvaluations { get; set; }
    public virtual ICollection<Notification>? Notifications { get; set; }
}