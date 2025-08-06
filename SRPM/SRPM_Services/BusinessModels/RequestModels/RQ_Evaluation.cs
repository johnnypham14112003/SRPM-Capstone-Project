using System.ComponentModel.DataAnnotations;

namespace SRPM_Services.BusinessModels.RequestModels;

public class RQ_Evaluation
{
    public Guid? Id { get; set; }

    public string? Code { get; set; }
    public string? Title { get; set; }
    public byte? TotalRate { get; set; }
    public string? Comment { get; set; }
    public DateTime CreateDate { get; set; } = DateTime.Now;
    [MaxLength(30)] public string Status { get; set; } = "created";

    // Foreign keys
    public Guid? ProjectId { get; set; }
    public Guid? AppraisalCouncilId { get; set; }
}