using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models;
public class ProjectMajor
{
    // Composite Primary Key
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!; // Many Projects -> Many Majors

    [Required]
    public Guid MajorId { get; set; }
    public Major Major { get; set; } = null!; // Many Majors -> Many Projects
}