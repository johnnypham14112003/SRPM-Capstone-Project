using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models
{
    public class Evaluation
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Range(0, 100)]
        public int TotalRate { get; set; } = 0;

        [Required]
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;

        public string Comment { get; set; }

        public bool IsApproved { get; set; } = false;

        [Required]
        [MaxLength(30)]
        public string Status { get; set; } = "Posted";

        public Guid? CreatorId { get; set; }
        public Guid? ProjectId { get; set; }
        public Guid? MilestoneId { get; set; }
        public Guid? TaskId { get; set; }

        // Navigation properties for relationships
        public virtual Account Creator { get; set; }
        public virtual Project Project { get; set; }
        public virtual Milestone Milestone { get; set; }
        public virtual Task Task { get; set; }

        // Many-to-many relationship with Criteria via CriteriaEvaluate
        public virtual ICollection<CriteriaEvaluate> CriteriaEvaluates { get; set; } = new List<CriteriaEvaluate>();

        // **New:** Evaluation to Notification relationship (one-to-many)
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
