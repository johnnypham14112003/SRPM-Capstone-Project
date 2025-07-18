using System.ComponentModel.DataAnnotations;

namespace SRPM_Services.BusinessModels.RequestModels.Query;

public class Q_IndividualEvaluation
{
    public string? KeyWord { get; set; }
    public byte? TotalRate { get; set; }
    public bool? IsApproved { get; set; }
    public bool? ReviewerResult { get; set; }
    public bool? IsAIReport { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? Status { get; set; }
    public Guid? EvaluationStageId { get; set; }
    public Guid? ReviewerId { get; set; }
    public byte SortBy { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
}