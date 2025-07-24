using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.BusinessModels.RequestModels
{
    public class RQ_ProjectTag
    {
        public List<string> Names { get; set; } = new();
        public Guid ProjectId { get; set; }
    }

}
