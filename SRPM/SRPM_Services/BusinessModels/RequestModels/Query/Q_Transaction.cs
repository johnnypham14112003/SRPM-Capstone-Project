namespace SRPM_Services.BusinessModels.RequestModels.Query;

public class Q_Transaction
{
    public string? KeyWord { get; set; }
    public Guid? ProjectId { get; set; }
    public Guid? EvaluationStageId { get; set; }
    public Guid? RequestPersonId { get; set; }
    public Guid? HandlePersonId { get; set; }
    public string? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public byte SortBy { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
}