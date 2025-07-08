using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.BusinessModels.Others
{
    public class PasswordEmailModel
    {
        public string UserName { get; set; }
        public string WebsiteURL { get; set; }
        public string OtpCode { get; set; }
    }

}
