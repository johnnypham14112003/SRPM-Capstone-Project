using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models;
public class Document
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();

    [Required] public string Name { get; set; } = null!;
    [Required, MaxLength(30)] public string Type { get; set; } = null!;//Final E-Doc | Ly lich khoa hoc
    [Required] public bool IsTemplate { get; set; } = false;
    public string? ContentHtml { get; set; }
    [Required] public DateTime UpdatedAt { get; set; } = DateTime.Now;
    [Required] public DateTime UploadAt { get; set; } = DateTime.Now;
    [Required, MaxLength(30)] public string Status { get; set; } = "created";

    // Foreign keys
    [Required] public Guid UploaderId { get; set; }
    public Guid? EditorId { get; set; }
    public Guid? ProjectId { get; set; }
    public Guid? EvaluationId { get; set; }
    public Guid? IndividualEvaluationId { get; set; }
    public Guid? TransactionId { get; set; }

    // Navigation properties
    public virtual UserRole Uploader { get; set; } = null!;
    public virtual UserRole? Editor { get; set; } = null!;
    public virtual Project? Project { get; set; }
    public virtual Evaluation? Evaluation { get; set; }
    public virtual IndividualEvaluation? IndividualEvaluation { get; set; }
    public virtual Transaction? Transaction { get; set; }
    public virtual ICollection<Signature>? Signatures { get; set; }
    public virtual ICollection<Notification>? Notifications { get; set; }
}