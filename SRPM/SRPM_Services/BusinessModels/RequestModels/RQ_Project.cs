using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.BusinessModels.RequestModels
{
    using System.ComponentModel.DataAnnotations;

    public class RQ_Project
    {
        [Required]
        public string EnglishTitle { get; set; } = null!;

        [Required]
        public string VietnameseTitle { get; set; } = null!;

        public string? Abbreviations { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Duration cannot be negative")]
        public int? Duration { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string? Description { get; set; }
        public string? RequirementNote { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Budget cannot be negative")]
        public decimal? Budget { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Progress cannot be negative")]
        public decimal? Progress { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Maximum member count cannot be negative")]
        public int? MaximumMember { get; set; }

        public string? Language { get; set; } = null!;

        [Required]
        public string Category { get; set; } = null!; // "basic" or "application/implementation"

        [Required]
        public string Type { get; set; } = null!;     // "school level" or "cooperate"

        public string? Genre { get; set; } = null!;   // "normal", "proposal", "propose"
    }



}
