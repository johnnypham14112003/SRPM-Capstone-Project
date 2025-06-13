using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models;

public class ProjectMajor
{
    // Composite Primary Key
    [Required] public Guid ProjectId { get; set; }
    [Required] public Guid MajorId { get; set; }

    // Navigation properties
    public Project Project { get; set; } = null!; // Many Projects -> Many Majors
    public Major Major { get; set; } = null!; // Many Majors -> Many Projects
}