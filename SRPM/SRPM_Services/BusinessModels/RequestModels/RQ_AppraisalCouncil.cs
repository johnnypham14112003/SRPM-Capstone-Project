namespace SRPM_Services.BusinessModels.RequestModels;

public class RQ_AppraisalCouncil
{
    public Guid? Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string Status { get; set; } = "created";
}