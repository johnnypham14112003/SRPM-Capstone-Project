using SRPM_Repositories.Models;

namespace SRPM_Services.BusinessModels.ResponseModels;

public class RS_IndividualEvaluation
{
    public Guid Id { get; set; }
    public byte? TotalRate { get; set; }
    public string? Comment { get; set; }
    public DateTime SubmittedAt { get; set; }
    public bool IsApproved { get; set; }
    public bool? ReviewerResult { get; set; }
    public bool IsAIReport { get; set; }
    public string Status { get; set; } = null!;

    public Guid EvaluationStageId { get; set; }
    public Guid? ReviewerId { get; set; }
    public Guid? ProjectId { get; set; }
    public Guid? MilestoneId { get; set; }
    public virtual ICollection<RS_Document>? Documents { get; set; }
    public virtual ICollection<Notification>? Notifications { get; set; }
}