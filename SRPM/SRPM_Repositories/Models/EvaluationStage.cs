using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models;
public class EvaluationStage
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [MaxLength(255)]
    public string? Name { get; set; }

    [Required]
    public int StageOrder { get; set; } = 1;

    [Required]
    [MaxLength(30)]
    public string Status { get; set; } = "draft";

    [Required]
    public Guid EvaluationId { get; set; }

    // Navigation properties
    public virtual Evaluation Evaluation { get; set; } = null!;
    public virtual AppraisalCouncil? Council { get; set; }
    public virtual ICollection<IndividualEvaluation>? IndividualEvaluations { get; set; }
    public virtual ICollection<Transaction>? Transactions { get; set; }
    public virtual ICollection<Notification>? Notifications { get; set; }
}