using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.BusinessModels.ResponseModels
{
    public class RS_Account
    {
        public Guid Id { get; set; }
        public string IdentityCode { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? AlternativeEmail { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool? Gender { get; set; }

        public string? Website { get; set; }
        public string? FacebookURL { get; set; }
        public string? LinkedInURL { get; set; }
        public string? AvatarURL { get; set; }
        public string? Bio { get; set; }
        public string? Degree { get; set; }
        public string? DegreeType { get; set; }
        public string? ProficiencyLevel { get; set; }
        public string? CompanyName { get; set; }

        public DateTime CreateTime { get; set; }
        public string Status { get; set; } = null!;
    }

}
