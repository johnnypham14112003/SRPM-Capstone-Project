using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.BusinessModels.ResponseModels
{
    public class RS_ProjectResult
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Url { get; set; }
        public DateTime AddedDate { get; set; }

        public List<RS_ResultPublish>? ResultPublishs { get; set; }
    }
    public class RS_ResultPublish
    {
        public Guid Id { get; set; }
        public string Url { get; set; } = null!;
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Publisher { get; set; }
        public DateTime? PublicationDate { get; set; }
        public string? AccessType { get; set; }
        public string? Tags { get; set; }
    }
}
