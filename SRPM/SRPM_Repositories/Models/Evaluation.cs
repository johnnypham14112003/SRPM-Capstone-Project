using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models;
public class Evaluation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code { get; set; } = null!;
    public string Title { get; set; } = null!;

    [Required]
    public byte TotalRate { get; set; } = 0;

    [Required]
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;

    public string? Comment { get; set; }

    [Required]
    public bool IsApproved { get; set; } = false;

    [Required]
    [MaxLength(30)]
    public string Type { get; set; } = "project";

    [Required]
    [MaxLength(30)]
    public string Status { get; set; } = "doing";

    [Required]
    public Guid ProjectId { get; set; }

    public Guid? MilestoneId { get; set; }
    public Guid? FinalDocId { get; set; }
    public Guid CouncilId { get; set; }

    // Navigation properties
    public virtual Project Project { get; set; } = null!;
    public virtual AppraisalCouncil? Council { get; set; }
    public virtual Milestone? Milestone { get; set; }
    public virtual Document? FinalDoc { get; set; }
    public virtual ICollection<EvaluationStage>? EvaluationStages { get; set; }
    public virtual ICollection<Notification>? Notifications { get; set; }
}