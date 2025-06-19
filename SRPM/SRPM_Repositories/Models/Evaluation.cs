using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models;

public class Evaluation
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();

    [Required] public string Code { get; set; } = null!;
    [Required] public string Title { get; set; } = null!;
    public byte? TotalRate { get; set; }
    public string? Comment { get; set; }
    [Required, MaxLength(30)] public string Phrase { get; set; } = "proposal";//report
    [Required, MaxLength(30)] public string Type { get; set; } = "project";//milestone
    [Required] public DateTime CreateDate { get; set; } = DateTime.Now;
    [Required, MaxLength(30)] public string Status { get; set; } = "created";

    // Foreign keys
    [Required] public Guid ProjectId { get; set; }
    public Guid? MilestoneId { get; set; }
    public Guid? AppraisalCouncilId { get; set; }

    // Navigation properties
    public virtual Project Project { get; set; } = null!;
    public virtual Milestone? Milestone { get; set; }
    public virtual AppraisalCouncil? AppraisalCouncil { get; set; }//Can create not by a council
    public virtual ICollection<Document>? Documents { get; set; }
    public virtual ICollection<EvaluationStage>? EvaluationStages { get; set; }
    public virtual ICollection<Notification>? Notifications { get; set; }
}