using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models
{
    public class ProjectTeam
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // Other properties (like IsSecretary, IsLeader, IsPrincipal, Status, ProjectId, AccountId)

        // Example properties:
        public bool IsSecretary { get; set; } = false;
        public bool IsLeader { get; set; } = false;
        public bool IsPrincipal { get; set; } = false;
        [Required]
        [MaxLength(30)]
        public string Status { get; set; } = "Pending";
        public Guid ProjectId { get; set; }
        public Guid AccountId { get; set; }

        // Navigation properties
        public virtual Project Project { get; set; }
        public virtual Account Account { get; set; }

        // **New:** Collection of Notifications related to this ProjectTeam
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
