namespace SRPM_Services.BusinessModels.RequestModels;

public class RQ_AppraisalCouncil
{
    public Guid? Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string Status { get; set; } = "created";
    public byte IncludeNo { get; set; } = 0; //1:Evaluations | 2:EvaluationStages | 3:Members | 12, 13, 23...
}