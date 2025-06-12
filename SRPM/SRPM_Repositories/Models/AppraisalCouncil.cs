using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models;

public class AppraisalCouncil
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(30)] public string Code { get; set; } = null!;
    [MaxLength(200)] public string? Name { get; set; }
    [Required] public DateTime CreatedAt { get; set; } = DateTime.Now;
    [Required] public DateTime UpdatedAt { get; set; } = DateTime.Now;
    [Required, MaxLength(30)] public string Status { get; set; } = "created";

    // Navigation properties
    public virtual ICollection<Evaluation>? Evaluations { get; set; }
    public virtual ICollection<EvaluationStage>? EvaluationStages { get; set; }
    public virtual ICollection<UserRole>? CouncilMembers { get; set; }
}
