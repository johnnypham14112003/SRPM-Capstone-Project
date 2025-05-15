using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models
{
    public class MemberTask
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid MemberId { get; set; }

        [Required]
        public Guid TaskId { get; set; }

        // Navigation properties for relationships
        public virtual Account Member { get; set; }
        public virtual Task Task { get; set; }

        // **New:** Collection of Notifications related to this MemberTask
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
