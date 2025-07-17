using SRPM_Repositories.Models;
using SRPM_Services.BusinessModels.ResponseModels;
using System.ComponentModel.DataAnnotations;

namespace SRPM_Services.BusinessModels.RequestModels;

public class RQ_IndividualEvaluation
{
    public Guid? Id { get; set; }

    public byte? TotalRate { get; set; }
    public string? Comment { get; set; }
    public DateTime SubmittedAt { get; set; } = DateTime.Now;
    public bool IsApproved { get; set; } = false;
    public bool? ReviewerResult { get; set; }
    public bool IsAIReport { get; set; } = false;
    [MaxLength(30)] public string Status { get; set; } = "created";

    // Foreign keys
    public Guid EvaluationStageId { get; set; }
    public Guid? ReviewerId { get; set; }
    public Guid? ProjectId { get; set; }
    public Guid? MilestoneId { get; set; }
    //public virtual ICollection<RS_Document>? Documents { get; set; }
    //public virtual ICollection<Notification>? Notifications { get; set; }
}