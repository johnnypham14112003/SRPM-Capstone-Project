using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.BusinessModels.RequestModels
{
    public class RQ_ProjectResult
    {
        public Guid? Id { get; set; } // null for create, value for update
        public string Name { get; set; } = null!;
        public string? Url { get; set; }
        public Guid ProjectId { get; set; }

        public List<RQ_ResultPublish>? ResultPublishs { get; set; }
    }
    public class RQ_ResultPublish
    {
        public Guid? Id { get; set; }
        public string Url { get; set; } = null!;
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Publisher { get; set; }
        public DateTime? PublicationDate { get; set; }
        public string? AccessType { get; set; }
        public string? Tags { get; set; }
    }
}
