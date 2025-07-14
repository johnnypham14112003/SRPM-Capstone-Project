namespace SRPM_Services.BusinessModels.RequestModels.Query;

public class Q_EvaluationStage
{
    public string? KeyWord { get; set; }
    public string? Status { get; set; }
    public Guid? EvaluationId { get; set; }
    public Guid? AppraisalCouncilId { get; set; }
    public byte SortBy { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
}