namespace SRPM_Services.BusinessModels.RequestModels.Query;

public class Q_Document
{
    public string? KeyWord { get; set; }
    public string? Type { get; set; }
    public byte SortBy { get; set; } = 1; //1: Name  | 2: CreateTime  | 3: UploaderId | 4: ProjectId | 5: EvaluationId | 6: IndividualEvaluationId | 7:TransactionId
    public string Status { get; set; } = "created";
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 5;
    public bool SearchByUploadDate = true;//False: search by upload date
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }

    // Foreign keys
    public Guid? UploaderId { get; set; }
    public Guid? ProjectId { get; set; }
    public Guid? EvaluationId { get; set; }
    public Guid? IndividualEvaluationId { get; set; }
    public Guid? TransactionId { get; set; }
}