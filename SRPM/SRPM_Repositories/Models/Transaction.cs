using System;
using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models
{
    public class Transaction
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(30)]
        public string Code { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        [Required]
        [MaxLength(30)]
        public string Type { get; set; }

        // Sender Information
        public string SenderAccount { get; set; }
        public string SenderName { get; set; }
        public string SenderBankName { get; set; }

        // Receiver Information
        public string ReceiverAccount { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverBankName { get; set; }

        [MaxLength(50)]
        public string TransferContent { get; set; }

        [Required]
        public DateTime RequestDate { get; set; } = DateTime.UtcNow;

        public DateTime? HandleDate { get; set; }

        [Required]
        public decimal FeeCost { get; set; } = 0;

        [Required]
        public decimal TotalMoney { get; set; } = 0;

        [Required]
        [MaxLength(30)]
        public string PayMethod { get; set; } = "Transfer";

        [Required]
        [MaxLength(30)]
        public string Status { get; set; } = "Complete";

        [Required]
        public Guid RequestPersonId { get; set; }

        [Required]
        public Guid HandlePersonId { get; set; }

        public Guid? ProjectId { get; set; }

        // Navigation properties
        public virtual Account RequestPerson { get; set; }
        public virtual Account HandlePerson { get; set; }
        public virtual Project Project { get; set; }

        // Notification relationship
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
