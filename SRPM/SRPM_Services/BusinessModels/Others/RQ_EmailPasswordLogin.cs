using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.BusinessModels.Others
{
    public class RQ_EmailPasswordLogin
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string SelectedRole { get; set; }
    }

}
