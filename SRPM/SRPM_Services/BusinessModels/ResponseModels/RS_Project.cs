namespace SRPM_Services.BusinessModels.ResponseModels;

public class RS_Project
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


    // 🧠 Related Entities
    public RS_UserRole? Creator { get; set; }
    //public RS_ResearchPaper? ResearchPaper { get; set; }
    public List<RS_UserRole>? Members { get; set; }
    public List<RS_Milestone>? Milestones { get; set; }
    public List<RS_Evaluation>? Evaluations { get; set; }
    public List<RS_ProjectSimilarity>? ProjectSimilarity { get; set; }
    public List<RS_MajorBrief> Majors { get; set; } = new();
    public List<RS_TagBrief>? ProjectTags { get; set; }
    public List<RS_Document>? Documents { get; set; }
    public List<RS_Transaction>? Transactions { get; set; }
}
public class RS_TagBrief
{
    public string Name { get; set; } = null!;
}