using System.ComponentModel.DataAnnotations;

namespace SRPM_Services.BusinessModels.RequestModels;

public class RQ_EvaluationStage
{
    public Guid? Id { get; set; }

    [MaxLength(255)] public string? Name { get; set; }
    public int? StageOrder { get; set; }
    [Required, MaxLength(30)] public string Phrase { get; set; } = null!;
    [Required, MaxLength(30)] public string Type { get; set; } = "project";//milestone
    [MaxLength(30)] public string Status { get; set; } = "created";

    // Foreign keys
    public Guid EvaluationId { get; set; }
    public Guid? MilestoneId { get; set; }
    public Guid? AppraisalCouncilId { get; set; }
}