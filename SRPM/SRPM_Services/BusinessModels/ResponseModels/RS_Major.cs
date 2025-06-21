using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.BusinessModels.ResponseModels
{
    public class RS_Major
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public Guid FieldId { get; set; }
    }
}
