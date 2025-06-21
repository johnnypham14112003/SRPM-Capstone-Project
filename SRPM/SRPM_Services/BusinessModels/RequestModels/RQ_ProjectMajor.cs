using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.BusinessModels.RequestModels
{
    public class RQ_ProjectMajor
    {
        public Guid ProjectId { get; set; }
        public Guid MajorId { get; set; }
    }

}
