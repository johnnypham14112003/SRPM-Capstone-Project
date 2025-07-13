using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models;

public class Project
{
    [Key] public Guid Id { get; set; }

    public string? LogoURL { get; set; }
    public string? PictureURL { get; set; }
    [Required, MaxLength(30)] public string Code { get; set; } = null!;
    [Required] public string EnglishTitle { get; set; } = null!;
    [Required] public string VietnameseTitle { get; set; } = null!;
    [MaxLength(100)] public string? Abbreviations { get; set; }
    public int? Duration { get; set; } = 1;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Description { get; set; }
    public string? EncodedDescription { get; set; }
    public string? RequirementNote { get; set; }
    [Required] public decimal Budget { get; set; } = 0m;// decimal suffix
    [Required] public decimal Progress { get; set; } = 0m;//100.00
    [Required] public int MaximumMember { get; set; } = 0;
    [Required, MaxLength(30)] public string Language { get; set; } = null!;
    [Required, MaxLength(30)] public string Category { get; set; } = null!;//basic || application/implementation
    [Required, MaxLength(30)] public string Type { get; set; } = null!;//school level || cooperate
    [Required, MaxLength(30)] public string Genre { get; set; } = null!;//normal || proposal || propose
    [Required] public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
    [Required, MaxLength(30)] public string Status { get; set; } = "created";

    // Foreign keys
    [Required] public Guid CreatorId { get; set; }

    // Navigation properties
    public virtual UserRole Creator { get; set; } = null!;
    public virtual ResearchPaper? ResearchPaper { get; set; } // 1 project only have 1 research paper
    public virtual ICollection<UserRole>? Members { get; set; }//1 project have many member
    public virtual ICollection<Milestone>? Milestones { get; set; }
    public virtual ICollection<Evaluation>? Evaluations { get; set; }
    public virtual ICollection<IndividualEvaluation>? IndividualEvaluations { get; set; }
    public virtual ICollection<ProjectMajor>? ProjectMajors { get; set; }
    public virtual ICollection<ProjectTag>? ProjectTags { get; set; }
    public virtual ICollection<Document>? Documents { get; set; }
    public virtual ICollection<Transaction>? Transactions { get; set; }
}