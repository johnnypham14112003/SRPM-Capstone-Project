using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models
{
    public class Project
    {
        [Key]
        public Guid Id { get; set; }

        public string LogoURL { get; set; }
        public string PictureURL { get; set; }
        public string DocURL { get; set; }

        [MaxLength(50)]
        public string Abbreviations { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string VietnameseTitle { get; set; }

        public string Description { get; set; }
        public string RequirementNote { get; set; }

        [Required]
        public decimal Budget { get; set; } = 0;

        [Required]
        public int Duration { get; set; } = 1; // Number of months

        [Required]
        public decimal Progress { get; set; } = 0;

        [Required]
        public int MaximumMember { get; set; } = 0;

        [Required]
        [MaxLength(30)]
        public string Language { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [Required]
        [MaxLength(30)]
        public string Status { get; set; } = "Created";

        // Foreign Key
        [Required]
        public Guid HostInstitutionId { get; set; }
        public Account HostInstitution { get; set; }

        // Navigation Properties
        public ICollection<Milestone> Milestones { get; set; } = new List<Milestone>();
        public ICollection<ProjectTag> ProjectTags { get; set; } = new List<ProjectTag>();
        public ICollection<ProjectTeam> ProjectTeams { get; set; } = new List<ProjectTeam>();
        public ICollection<ProjectField> ProjectFields { get; set; } = new List<ProjectField>();
        public ICollection<ProjectMajor> ProjectMajors { get; set; } = new List<ProjectMajor>();
        public ICollection<Evaluation> Evaluations { get; set; } = new List<Evaluation>();
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
