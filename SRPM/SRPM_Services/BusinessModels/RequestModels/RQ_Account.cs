using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.BusinessModels.RequestModels
{
    public class RQ_Account
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? AlternativeEmail { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool? Gender { get; set; }

        // Optional investigator / host info
        public string? Bio { get; set; }
        public string? Degree { get; set; }
        public string? DegreeType { get; set; }
        public string? ProficiencyLevel { get; set; }
        public string? CompanyName { get; set; }

        // External links
        public string? Website { get; set; }
        public string? FacebookURL { get; set; }
        public string? LinkedInURL { get; set; }
        public string? AvatarURL { get; set; }

        public string Status { get; set; } = "created";

        public Guid? MajorId { get; set; }
    }


}
