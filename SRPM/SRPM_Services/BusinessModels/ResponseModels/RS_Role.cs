using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.BusinessModels.ResponseModels
{

    public class RS_Role
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsGroupRole { get; set; }
        public string Status { get; set; } = null!;
    }

}
