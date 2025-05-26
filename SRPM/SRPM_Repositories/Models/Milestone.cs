using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SRPM_Repositories.Models;
public class Milestone
{
    [Key]
    public Guid Id { get; set; }

    [MaxLength(30)]
    public string Code { get; set; }

    [Required, MaxLength(255)]
    public string Name { get; set; }

    public string Description { get; set; }
    public string Objective { get; set; }
    public decimal Cost { get; set; } = 0;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public DateTime CreatedAt { get; set; }

    [Required, MaxLength(30)]
    public string Status { get; set; } = "draft";

    [Required]
    public Guid ProjectId { get; set; }
    public Project Project { get; set; }

    [Required]
    public Guid CreateBy { get; set; }
    [ForeignKey("CreateBy")]
    public Account CreateByAccount { get; set; }

    public ICollection<Task> Tasks { get; set; }
    public ICollection<Evaluation> Evaluations { get; set; }
}