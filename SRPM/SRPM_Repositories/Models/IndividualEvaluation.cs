using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models;

public class IndividualEvaluation
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = null!;
    public byte? TotalRate { get; set; }
    public string? Comment { get; set; }
    [Required] public DateTime SubmittedAt { get; set; } = DateTime.Now;
    [Required] public bool IsApproved { get; set; } = false;
    public bool? ReviewerResult { get; set; }
    [Required] public bool IsAIReport { get; set; } = false;
    [Required,MaxLength(30)] public string Status { get; set; } = "created";

    // Foreign keys
    [Required] public Guid EvaluationStageId { get; set; }
    public Guid? ReviewerId { get; set; }

    // Navigation properties
    public virtual EvaluationStage EvaluationStage { get; set; } = null!;
    public virtual UserRole? Reviewer { get; set; }//Can create not by a real person
    public virtual ICollection<ProjectSimilarity>? ProjectsSimilarity { get; set; }
    public virtual ICollection<Document>? Documents { get; set; }
    public virtual ICollection<Notification>? Notifications { get; set; }
}