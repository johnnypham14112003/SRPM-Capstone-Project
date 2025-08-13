using System.ComponentModel.DataAnnotations;

namespace SRPM_Services.BusinessModels.ResponseModels;

public class RS_Document
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = null!;
    [MaxLength(30)] public string Type { get; set; } = null!;//Final E-Doc | Ly lich khoa hoc
    public bool IsTemplate { get; set; } = false;
    public string? ContentHtml { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime UploadAt { get; set; }
    [MaxLength(30)] public string Status { get; set; } = null!;

    // Foreign keys
    public Guid? EditorId { get; set; }
    public Guid UploaderId { get; set; }
    public Guid? ProjectId { get; set; }
    public Guid? EvaluationId { get; set; }
    public Guid? IndividualEvaluationId { get; set; }
    public Guid? TransactionId { get; set; }
    public virtual ICollection<RS_Signature>? Signatures { get; set; }
}