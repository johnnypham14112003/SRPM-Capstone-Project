using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.BusinessModels.ResponseModels
{
    public class RS_ProjectMajor
    {
        public Guid ProjectId { get; set; }
        public Guid MajorId { get; set; }

        public RS_ProjectBrief? Project { get; set; }
        public RS_MajorBrief? Major { get; set; }
    }

    public class RS_ProjectBrief
    {
        public string Code { get; set; } = null!;
        public string EnglishTitle { get; set; } = null!;
        public string VietnameseTitle { get; set; } = null!;
    }

    public class RS_MajorBrief
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public RS_FieldBrief? Field { get; set; }
    }

    public class RS_FieldBrief
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
    }

}
