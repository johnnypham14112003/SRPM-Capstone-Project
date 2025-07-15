using System.ComponentModel.DataAnnotations;

namespace SRPM_Services.BusinessModels.RequestModels;

public class RQ_Evaluation
{
    public Guid? Id { get; set; }

    public string? Code { get; set; }
    public string? Title { get; set; }
    public byte? TotalRate { get; set; }
    public string? Comment { get; set; }
    [MaxLength(30)] public string Phrase { get; set; } = "proposal";//report
    [MaxLength(30)] public string Type { get; set; } = "project";//milestone
    public DateTime CreateDate { get; set; } = DateTime.Now;
    [MaxLength(30)] public string Status { get; set; } = "created";

    // Foreign keys
    public Guid? ProjectId { get; set; }
    public Guid? MilestoneId { get; set; }
    public Guid? AppraisalCouncilId { get; set; }
}