using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.BusinessModels.RequestModels
{
    public class RQ_Project
    {
        public string EnglishTitle { get; set; } = null!;
        public string VietnameseTitle { get; set; } = null!;
        public string? Abbreviations { get; set; }
        public int? Duration { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }
        public string? RequirementNote { get; set; }
        public decimal? Budget { get; set; }
        public decimal? Progress { get; set; }
        public int? MaximumMember { get; set; }
        public string? Language { get; set; } = null!;

        // Enum-like fields
        public string Category { get; set; } = null!; // "basic" or "application/implementation"
        public string Type { get; set; } = null!;     // "school level" or "cooperate"
        public string? Genre { get; set; } = null!;    // "normal", "proposal", "propose"
    }



}
