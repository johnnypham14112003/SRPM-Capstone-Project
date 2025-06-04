using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SRPM_Repositories.Models;
public class Project
{
    [Key]
    public Guid Id { get; set; }
    public string LogoURL { get; set; }
    public string PictureURL { get; set; }
    [Required]
    [MaxLength(30)]
    public string Code { get; set; } = string.Empty;
    [Required]
    public string EnglishTitle { get; set; }
    [Required]
    public string VietnameseTitle { get; set; }
    public int Duration { get; set; } = 1;  // month
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    [MaxLength(50)]
    public string Abbreviations { get; set; }
    public string Description { get; set; }
    public string RequirementNote { get; set; }
    public decimal Budget { get; set; } = 0;
    public decimal Progress { get; set; } = 0;
    public int MaximumMember { get; set; } = 0;
    [Required, MaxLength(30)]
    public string Language { get; set; }
    [Required, MaxLength(30)]
    public string Category { get; set; } = "basic";
    [Required, MaxLength(30)]
    public string Type { get; set; } = "normal";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    [Required, MaxLength(30)]
    public string Status { get; set; } = "draft";
    // Foreign keys
    [Required]
    public Guid HostInstitutionId { get; set; }
    public Account HostInstitution { get; set; }

    [Required]
    public Guid CreateBy { get; set; }
    [ForeignKey("CreateBy")]
    public Account CreatedByAccount { get; set; }

    // Navigation
    public virtual ResearchPaper ResearchPaper { get; set; }
    public virtual ICollection<ProjectTag> ProjectTags { get; set; }
    public virtual ICollection<ProjectMajor> ProjectMajors { get; set; }
    public virtual ICollection<Milestone> Milestones { get; set; }
    public virtual ICollection<Document> Documents { get; set; }
    public virtual ICollection<UserRole> UserRoles { get; set; }
    public virtual ICollection<Evaluation> Evaluations { get; set; }
}