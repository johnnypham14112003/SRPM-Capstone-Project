using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.BusinessModels.Others
{
    public class RQ_MajorQuery
    {
        public Guid? FieldId { get; set; }
        public string? Name { get; set; }

        public string? SortBy { get; set; } = "name"; // default sorting field
        public bool Desc { get; set; } = false; // descending toggle

        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }


}
