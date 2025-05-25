using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models
{
    public class Evaluation
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public byte TotalRate { get; set; } = 0;

        [Required]
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;

        public string? Comment { get; set; }

        [Required]
        public bool IsApproved { get; set; } = false;

        [Required]
        [MaxLength(30)]
        public string Type { get; set; } = "project";

        [Required]
        [MaxLength(30)]
        public string Status { get; set; } = "doing";

        [Required]
        public Guid CouncilId { get; set; }

        [Required]
        public Guid ProjectId { get; set; }

        public Guid? MilestoneId { get; set; }
        public Guid? FinalDocId { get; set; }

        // Navigation properties
        public virtual AppraisalCouncil Council { get; set; } = null!;
        public virtual Project Project { get; set; } = null!;
        public virtual Milestone? Milestone { get; set; }
        public virtual Document? FinalDoc { get; set; }
        public virtual ICollection<EvaluationStage> EvaluationStages { get; set; } = new List<EvaluationStage>();
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
