using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models;
public class UserRole
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid AccountId { get; set; }

    [Required]
    public Guid RoleId { get; set; }

    [Required]
    [MaxLength(30)]
    public string Code { get; set; } = string.Empty;

    public string? GroupName { get; set; }
    public bool IsOfficial { get; set; } = false;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [Required, MaxLength(30)]
    public string Status { get; set; } = "draft";

    public Guid? ProjectId { get; set; }
    public Guid? CouncilId { get; set; }

    // Navigation properties
    public virtual Account Account { get; set; } = null!;
    public virtual Role Role { get; set; } = null!;
    //---------
    public virtual AppraisalCouncil? Council { get; set; }
    public virtual Project? Project { get; set; }//This user belong to which project
    public virtual ICollection<Project>? HadProjects { get; set; }//HostInstitute have many project || CreatedProject
    public virtual ICollection<Task>? CreatedTasks { get; set; }//who create those task
    public virtual ICollection<Milestone>? CreatedMilestones { get; set; }// who create those milestone
    public virtual ICollection<MemberTask>? MemberTasks { get; set; }//n task - 1 member
    public virtual ICollection<Document>? UploadedDocuments { get; set; }
    public virtual ICollection<Transaction>? Transactions { get; set; }
    public virtual ICollection<Notification>? Notifications { get; set; }
}