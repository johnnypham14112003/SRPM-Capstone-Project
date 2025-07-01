using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.BusinessModels.RequestModels
{
    public class RQ_MemberTask
    {
        [Range(0, 100)]
        public decimal Progress { get; set; } = 0m;

        [Range(0, int.MaxValue)]
        public int Overdue { get; set; } = 0;

        public string? Note { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public Guid MemberId { get; set; }
        public Guid TaskId { get; set; }
    }



}
