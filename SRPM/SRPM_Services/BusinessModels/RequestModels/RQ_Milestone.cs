using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.BusinessModels.RequestModels
{
    public class RQ_Milestone
    {
        [Required]
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string? Objective { get; set; }
        [Required]
        public decimal Cost { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Type { get; set; } = "normal";
        public Guid ProjectId { get; set; }
        public Guid CreatorId { get; set; }
    }

}
