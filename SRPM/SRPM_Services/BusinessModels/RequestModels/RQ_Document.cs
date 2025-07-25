using SRPM_Repositories.Models;
using System.ComponentModel.DataAnnotations;

namespace SRPM_Services.BusinessModels.RequestModels;

public class RQ_Document
{
    public Guid? Id { get; set; }

    public string Name { get; set; } = null!;
    [MaxLength(30)] public string? Type { get; set; } //System | Final E-Doc | Ly lich khoa hoc
    public bool IsTemplate { get; set; } = false;
    public string? ContentHtml { get; set; }
    [Required] public DateTime UpdatedAt { get; set; } = DateTime.Now;
    [Required] public DateTime UploadAt { get; set; } = DateTime.Now;
    [MaxLength(30)] public string Status { get; set; } = "created";

    // Foreign keys
    public Guid? UploaderId { get; set; }
    public Guid? EditorId { get; set; }
    public Guid? ProjectId { get; set; }
    public Guid? EvaluationId { get; set; }
    public Guid? IndividualEvaluationId { get; set; }
    public Guid? TransactionId { get; set; }

    //public virtual ICollection<RQ_Signature>? Signatures { get; set; }
}