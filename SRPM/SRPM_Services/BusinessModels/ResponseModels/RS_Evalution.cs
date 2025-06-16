using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.BusinessModels.ResponseModels
{
    public class RS_Evaluation
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = null!;
        public string Title { get; set; } = null!;
        public byte? TotalRate { get; set; }
        public string? Comment { get; set; }
        public string Phrase { get; set; } = null!;
        public string Type { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public string Status { get; set; } = null!;
    }

}
