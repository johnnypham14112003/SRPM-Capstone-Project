namespace SRPM_Services.BusinessModels.RequestModels.Query;

public class Q_AppraisalCouncil
{
    public string? KeyWord { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 5;
    public byte SortBy { get; set; } = 1; //1: Code  | 2: Name  | 3: CreateTime
    public string Status { get; set; } = "created";
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}