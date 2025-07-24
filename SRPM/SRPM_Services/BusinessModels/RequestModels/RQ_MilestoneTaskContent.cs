namespace SRPM_Services.BusinessModels.RequestModels;

public class RQ_MilestoneTaskContent
{
    public string SectionTitle { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Objective { get; set; } = null!;
    public string CostEstimate { get; set; } = null!;
    public string TimeEstimate { get; set; } = null!;
}