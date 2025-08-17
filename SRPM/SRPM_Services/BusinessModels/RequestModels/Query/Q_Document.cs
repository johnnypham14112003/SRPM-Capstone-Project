namespace SRPM_Services.BusinessModels.RequestModels.Query;

public class Q_Document
{
    public string? KeyWord { get; set; }
    public string? Type { get; set; }
    public bool IsTemplate { get; set; } = true;
    public string? Status { get; set; }
    public byte SortBy { get; set; } = 1; // 1=UpdatedAt↓, 2=UpdatedAt↑, 3=UploadAt↓, 4=UploadAt↑, 5=UploaderId↑, 6=ProjectId↑, 7=EvaluationId↑, 8=IndividualEvaluationId↑, 9=TransactionId↑, 10=EditorId↑, 11=Name↓, ...=Name↑
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }

    // Foreign keys
    public Guid? UploaderId { get; set; }
    public Guid? EditorId { get; set; }
    public Guid? ProjectId { get; set; }
    public Guid? EvaluationId { get; set; }
    public Guid? IndividualEvaluationId { get; set; }
    public Guid? TransactionId { get; set; }
}