using System.ComponentModel.DataAnnotations;

namespace SRPM_Services.BusinessModels.ResponseModels;

public class RS_IndividualEvaluation
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
    public byte? TotalRate { get; set; }
    public string? Comment { get; set; }
    public DateTime SubmittedAt { get; set; }
    public bool IsApproved { get; set; } = false;
    public bool? ReviewerResult { get; set; }
    public bool IsAIReport { get; set; } = false;
    [MaxLength(30)] public string Status { get; set; } = null!;

    // Foreign keys
    public Guid EvaluationStageId { get; set; }
    public Guid? ReviewerId { get; set; }
    public virtual ICollection<RS_Document>? Documents { get; set; }
    public virtual ICollection<RS_ProjectSimilarityResult>? ProjectsSimilarityResult { get; set; }
}