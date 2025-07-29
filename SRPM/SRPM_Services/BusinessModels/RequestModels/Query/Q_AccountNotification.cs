namespace SRPM_Services.BusinessModels.RequestModels.Query;

public class Q_AccountNotification
{
    public string? Email { get; set; }
    public string? KeyWord { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public bool IsRead { get; set; } = false;
    public string? Type { get; set; }
    public string? Status { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 5;
}
