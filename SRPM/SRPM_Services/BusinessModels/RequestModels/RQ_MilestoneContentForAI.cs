namespace SRPM_Services.BusinessModels.RequestModels;
public class RQ_MilestoneContentForAI
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string? Objective { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}