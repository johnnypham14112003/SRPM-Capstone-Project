using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models
{
    public class Milestone
    {
        [Key]
        public Guid Id { get; set; }

        [MaxLength(30)]
        public string Code { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        public string Description { get; set; }
        public string Objective { get; set; }

        [Required]
        public decimal Cost { get; set; } = 0;

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [Required]
        [MaxLength(30)]
        public string Status { get; set; } = "Draft";

        // Foreign Key
        [Required]
        public Guid ProjectId { get; set; }
        public Project Project { get; set; } // One Project -> Many Milestones

        // Navigation Properties
        public ICollection<Task> Tasks { get; set; } = new List<Task>();
        public ICollection<Evaluation> Evaluations { get; set; } = new List<Evaluation>();
    }
}
