namespace SRPM_Services.BusinessModels.ResponseModels;

public class RS_AccountNotification
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Type { get; set; } = null!;
    public DateTime CreateDate { get; set; } = DateTime.Now;
    public Guid? TypeObjectId { get; set; }

    //For AccountNotification
    public Guid? AccountId { get; set; }
    public bool IsRead { get; set; } = false;

    //For manager
    public bool IsGlobalSend { get; set; } = false;
    public string Status { get; set; } = null!;
}