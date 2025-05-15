using System;
using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models
{
    public class Notification
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        [Required]
        [MaxLength(30)]
        public string Type { get; set; }

        public bool IsSystemSend { get; set; } = false;

        public bool IsRead { get; set; } = false;

        [Required]
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(30)]
        public string Status { get; set; } = "Unread";

        public Guid? SenderId { get; set; }
        public Guid? ReceiverId { get; set; }
        public Guid? ProjectId { get; set; }
        public Guid? ProjectTeamId { get; set; }
        public Guid? MemberTaskId { get; set; }
        public Guid? EvaluationId { get; set; }
        public Guid? TransactionId { get; set; }

        // Navigation properties
        public virtual Account Sender { get; set; }
        public virtual Account Receiver { get; set; }
        public virtual Project Project { get; set; }
        public virtual ProjectTeam ProjectTeam { get; set; }
        public virtual MemberTask MemberTask { get; set; }
        public virtual Evaluation Evaluation { get; set; }
        public virtual Transaction Transaction { get; set; }
    }
}
