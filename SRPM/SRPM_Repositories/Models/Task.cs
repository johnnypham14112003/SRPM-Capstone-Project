using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models
{
    public class Task
    {
        [Key]
        public Guid Id { get; set; }

        [MaxLength(30)]
        public string Code { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? DeliveryDate { get; set; }

        [MaxLength(30)]
        public string Priority { get; set; }

        [Required]
        public decimal Progress { get; set; } = 0;

        [Required]
        public int Overdue { get; set; } = 0;

        public string Note { get; set; }

        [Required]
        [MaxLength(30)]
        public string Status { get; set; } = "Draft"; // Applied, Working, Done, Overdue

        // Foreign Key
        [Required]
        public Guid MilestoneId { get; set; }
        public Milestone Milestone { get; set; } // One Milestone -> Many Tasks

        // Navigation Properties
        public ICollection<MemberTask> MemberTasks { get; set; } = new List<MemberTask>();
        public ICollection<Evaluation> Evaluations { get; set; } = new List<Evaluation>();
    }
}
