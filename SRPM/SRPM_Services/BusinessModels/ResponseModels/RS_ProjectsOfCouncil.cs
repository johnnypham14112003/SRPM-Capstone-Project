namespace SRPM_Services.BusinessModels.ResponseModels;

public class RS_ProjectsOfCouncil
{
    public Guid Id { get; set; }
    public string? LogoURL { get; set; }
    public string? PictureURL { get; set; }
    public string Code { get; set; } = null!;
    public string EnglishTitle { get; set; } = null!;
    public string VietnameseTitle { get; set; } = null!;
    public string? Abbreviations { get; set; }
    public int? Duration { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Description { get; set; }
    public string? RequirementNote { get; set; }
    public decimal Budget { get; set; }
    public decimal Progress { get; set; }
    public int MaximumMember { get; set; }
    public string Language { get; set; } = null!;
    public string Category { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string Genre { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string Status { get; set; } = null!;
    public Guid CreatorId { get; set; }
    public List<RS_TagBrief> ProjectTags { get; set; }
    public List<RS_Project> Proposals { get; set; }
}