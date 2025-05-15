using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models
{
    public class Account
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string IdentityCode { get; set; } // Unique, required identifier (e.g., SE172789...)

        [Required]
        [MaxLength(50)]
        public string FullName { get; set; }

        [Required]
        [MaxLength(320)]
        public string Email { get; set; }

        [Required]
        [MaxLength(320)]
        public string AlternativeEmail { get; set; }

        [MaxLength(15)]
        public string PhoneNumber { get; set; }

        public string Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool Gender { get; set; }

        // Authentication & Profile Data
        public string FirebaseUid { get; set; }
        public string Website { get; set; }
        public string FacebookURL { get; set; }
        public string LinkedInURL { get; set; }
        public string AvatarURL { get; set; }
        public string Password { get; set; } // Stored as hashed values

        // Research Role & Affiliation
        public string SelfDescription { get; set; }
        [MaxLength(250)]
        public string Degree { get; set; }
        [MaxLength(50)]
        public string DegreeType { get; set; }
        [MaxLength(50)]
        public string ProficiencyLevel { get; set; }
        [MaxLength(150)]
        public string CompanyName { get; set; }

        public DateTime CreateTime { get; set; } = DateTime.UtcNow;
        public DateTime? DeleteTime { get; set; }

        [Required]
        [MaxLength(30)]
        public string Role { get; set; } = "Member"; // Principal Investigator, Host Institution, etc.

        [Required]
        [MaxLength(30)]
        public string Status { get; set; } = "Created";

        // Foreign Key Relationships
        public Guid FieldId { get; set; }
        public Field Field { get; set; }

        public Guid MajorId { get; set; }
        public Major Major { get; set; }

        // Navigation Properties
        public ICollection<ResearchPaper> ResearchPapers { get; set; } = new List<ResearchPaper>();
        public ICollection<ProjectTeam> ProjectTeams { get; set; } = new List<ProjectTeam>();
        public ICollection<Transaction> RequestedTransactions { get; set; } = new List<Transaction>();
        public ICollection<Transaction> HandledTransactions { get; set; } = new List<Transaction>();
        public ICollection<Evaluation> Evaluations { get; set; } = new List<Evaluation>();
        public ICollection<MemberTask> MemberTasks { get; set; } = new List<MemberTask>();
        public ICollection<Notification> SentNotifications { get; set; } = new List<Notification>();
        public ICollection<Notification> ReceivedNotifications { get; set; } = new List<Notification>();
        public ICollection<OTPCode> OTPCodes { get; set; } = new List<OTPCode>();
    }
}
