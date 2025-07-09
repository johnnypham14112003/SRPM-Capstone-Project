using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.BusinessModels.Others
{
    public class RQ_AccountQuery
    {
        public string? IdentityCode { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Status { get; set; }

        public string? SortBy { get; set; }  // "fullname", "email", or "identitycode"
        public bool Desc { get; set; } = false;

        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

}
