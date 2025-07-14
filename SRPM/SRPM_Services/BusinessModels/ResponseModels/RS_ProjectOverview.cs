using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.BusinessModels.ResponseModels
{
    public class RS_ProjectOverview
    {
        public Guid Id { get; set; }
        public string EnglishTitle { get; set; } = null!;
        public string VietnameseTitle { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string Genre { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
            public string? PictureUrl { get; set; } 
        public string? Description { get; set; }
        public string? RequirementNote { get; set; }
        public decimal Progress { get; set; } = 0m;
        public string Language { get; set; } = null!;
    }

}
