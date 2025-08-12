namespace SRPM_Services.BusinessModels.RequestModels;

public class RQ_ProjectContentForAI
{
    public string EnglishTitle { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? RequirementNote { get; set; }
    public int MaximumMember { get; set; } = 1;
    public string Category { get; set; } = null!;//basic || application/implementation
    public string Type { get; set; } = null!;//school level || cooperate
    public string Genre { get; set; } = null!;//normal || proposal || propose
    public string? DocumentContent { get; set; }

    public virtual ICollection<RQ_MilestoneContentForAI>? MilestoneContents { get; set; }
}