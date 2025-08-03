using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models;
public class UserRole
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();//For notification

    [Required, MaxLength(30)] public string Code { get; set; } = null!;
    public string? GroupName { get; set; }
    public bool IsOfficial { get; set; } = false;//for query between multiple group join
    public DateTime? ExpireDate { get; set; }
    [Required] public DateTime CreatedAt { get; set; } = DateTime.Now;
    [Required, MaxLength(30)] public string Status { get; set; } = "created";

    // Foreign keys
    [Required] public Guid AccountId { get; set; }
    [Required] public Guid RoleId { get; set; }
    public Guid? ProjectId { get; set; }
    public Guid? AppraisalCouncilId { get; set; }

    // Navigation properties
    public virtual Account Account { get; set; } = null!;
    public virtual Role Role { get; set; } = null!;
    public virtual Project? Project { get; set; }//This user is a member of which project
    public virtual AppraisalCouncil? AppraisalCouncil { get; set; }
    public virtual ICollection<Document>? UploadedDocuments { get; set; }
    public virtual ICollection<Document>? ModifiedDocuments { get; set; }
    public virtual ICollection<Signature>? Signatures { get; set; }
    public virtual ICollection<IndividualEvaluation>? IndividualEvaluations { get; set; }
    public virtual ICollection<Project>? CreatedProjects { get; set; }//HostInstitute have many project || CreatedProject
    public virtual ICollection<Milestone>? CreatedMilestones { get; set; }// who created those milestones
    public virtual ICollection<Task>? CreatedTasks { get; set; }//who create those task
    public virtual ICollection<MemberTask>? MemberTasks { get; set; }//n task - 1 member
    public virtual ICollection<Transaction>? RequestTransactions { get; set; }
    public virtual ICollection<Transaction>? HandleTransactions { get; set; }
    public virtual ICollection<Notification>? Notifications { get; set; }//For group notification
}