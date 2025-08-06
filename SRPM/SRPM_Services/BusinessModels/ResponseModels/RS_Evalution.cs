namespace SRPM_Services.BusinessModels.ResponseModels;

public class RS_Evaluation
{
    public Guid? Id { get; set; }

    public string? Code { get; set; }
    public string? Title { get; set; }
    public byte? TotalRate { get; set; }
    public string? Comment { get; set; }
    public DateTime CreateDate { get; set; } = DateTime.Now;
    public string Status { get; set; } = null!;

    // Foreign keys
    public Guid ProjectId { get; set; }
    public Guid? AppraisalCouncilId { get; set; }

    // Navigation properties
    public virtual ICollection<RS_Document>? Documents { get; set; }
    public virtual ICollection<RS_EvaluationStage>? EvaluationStages { get; set; }
}