using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.BusinessModels.ResponseModels
{
    public class RS_EvaluationStage
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public int StageOrder { get; set; }
        public string Status { get; set; } = null!;
        public Guid EvaluationId { get; set; }
        public Guid? AppraisalCouncilId { get; set; }
    }

}
