using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.BusinessModels.Others
{
    public class RQ_ProjectQuery
    {
        public string? Code { get; set; }
        public string? Title { get; set; } 
        public string? Category { get; set; }
        public string? Type { get; set; }
        public string? Genre { get; set; }
        public string? Language { get; set; }
        public string? Status { get; set; } 

        public Guid? CreatorId { get; set; }

        public int? Duration { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public decimal? MinBudget { get; set; }
        public decimal? MaxBudget { get; set; }
        public decimal? Progress { get; set; }

        public string? SortBy { get; set; } // e.g. "englishTitle", "vietnameseTitle", "duration", "budget"
        public bool Desc { get; set; } = false;

        public bool IncludeCreator { get; set; } = false;
        public bool IncludeMembers { get; set; } = false;
        public bool IncludeMilestones { get; set; } = false;
        public bool IncludeResearchPaper { get; set; } = false;

        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

}
