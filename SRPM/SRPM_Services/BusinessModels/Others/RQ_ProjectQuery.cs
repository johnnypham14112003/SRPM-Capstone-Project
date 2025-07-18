using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.BusinessModels.Others
{
    public class RQ_ProjectQuery
    {
        public string? Title { get; set; }
        public string? Category { get; set; }
        public string? Type { get; set; }
        public string? Genre { get; set; }
        public string? Status { get; set; }
        public string? Language { get; set; }

        public Guid? MajorId { get; set; }
        public Guid? FieldId { get; set; }
        public List<string>? TagNames { get; set; }


        public string? SortBy { get; set; }
        public bool Desc { get; set; } = false;


        public bool IncludeCreator { get; set; } = false;
        public bool IncludeMembers { get; set; } = false;
        public bool IncludeMilestones { get; set; } = false;
        public bool IncludeEvaluations { get; set; } = false;
        public bool IncludeDocuments { get; set; } = false;
        public bool IncludeProjectSimilarity { get; set; } = false;
        public bool IncludeTransactions { get; set; } = false;


        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }


}
