namespace SRPM_Services.BusinessModels.RequestModels.Query;

public class Q_Evaluation
{
    public string? KeyWord { get; set; }
    public byte SortBy { get; set; } = 1; //1: Name  2:Rating | 3: CreateTime  | 4: phrase | 5: type | 6: ProjectId | 7: MilestoneId | 8:AppraisalCouncilId
    public byte? Rating { get; set; }
    public string? Status { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 5;
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }

    // Foreign keys
    public Guid? ProjectId { get; set; }
    public Guid? AppraisalCouncilId { get; set; }
}