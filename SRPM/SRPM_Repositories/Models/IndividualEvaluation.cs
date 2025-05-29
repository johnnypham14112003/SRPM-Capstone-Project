using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SRPM_Repositories.Models;
public class IndividualEvaluation
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public byte TotalRate { get; set; } = 0;

    public string? Comment { get; set; }

    [Required]
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public bool IsApproved { get; set; } = false;

    [Required]
    [MaxLength(30)]
    public string Status { get; set; } = "submitted";

    [Required]
    public Guid EvaluationStageId { get; set; }

    [Required]
    public Guid ReviewerId { get; set; }

    public Guid? ProjectId { get; set; }
    public Guid? MilestoneId { get; set; }

    // Navigation properties
    [ForeignKey(nameof(EvaluationStageId))]
    public EvaluationStage EvaluationStage { get; set; }
    [ForeignKey(nameof(ReviewerId))]
    public Account Reviewer { get; set; }
    [ForeignKey(nameof(ProjectId))]
    public virtual Project? Project { get; set; }
    [ForeignKey(nameof(MilestoneId))]
    public virtual Milestone? Milestone { get; set; }
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}