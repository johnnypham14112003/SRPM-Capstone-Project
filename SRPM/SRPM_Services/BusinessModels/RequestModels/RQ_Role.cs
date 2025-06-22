using SRPM_Services.Extensions.Enumerables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.BusinessModels.RequestModels
{
    public class RQ_Role
    {
        public string Name { get; set; } = null!;
        public bool IsGroupRole { get; set; }
        public string Status { get; set; } = "created";
    }

}
