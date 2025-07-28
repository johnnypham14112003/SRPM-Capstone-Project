using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.BusinessModels.RequestModels
{
    public class RQ_UserRole
    {
        [Required]public Guid AccountId { get; set; }
        [Required] public Guid RoleId { get; set; }
        public Guid? ProjectId { get; set; }
        public Guid? AppraisalCouncilId { get; set; }
    }


}
