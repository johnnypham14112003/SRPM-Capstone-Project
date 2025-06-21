using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.BusinessModels.RequestModels
{
    public class RQ_Major
    {
        public string Name { get; set; } = null!;
        public Guid FieldId { get; set; }
    }

}
