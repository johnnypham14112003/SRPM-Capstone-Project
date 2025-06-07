using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models
{
    public class Account
    {
        [Key]
        public Guid Id { get; set; }

        public string? FirebaseUid { get; set; }
        public string? Website { get; set; }
        public string? FacebookURL { get; set; }
        public string? LinkedInURL { get; set; }

        // Basic Info
        public string? AvatarURL { get; set; }

        [Required, MaxLength(30)]
        public string IdentityCode { get; set; } = null!; // e.g. se172789...

        [Required, MaxLength(50)]
        public string FullName { get; set; } = null!;

        [Required, MaxLength(320)]
        public string Email { get; set; } = null!;

        [MaxLength(320)]
        public string? AlternativeEmail { get; set; }

        public string? Password { get; set; } // hash code

        [MaxLength(15)]
        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool? Gender { get; set; }

        // Principal Investigator
        public string? SelfDescription { get; set; }
        public string? Degree { get; set; }

        [MaxLength(50)]
        public string? DegreeType { get; set; }

        public string? ProficiencyLevel { get; set; }

        // Host Institution
        [MaxLength(150)]
        public string? CompanyName { get; set; }

        [Required]
        [MaxLength(30)]
        public string Status { get; set; } = "created";

        public DateTime CreateTime { get; set; }
        public DateTime? DeleteTime { get; set; }

        // Foreign key to Major, if applicable
        public Guid? MajorId { get; set; }
        public Major? Major { get; set; }

        // Navigation properties
        public virtual ICollection<OTPCode>? OTPCodes { get; set; }
        public virtual ICollection<UserRole>? UserRoles { get; set; }
        public virtual ICollection<AccountNotification>? AccountNotifications { get; set; }
    }
}
