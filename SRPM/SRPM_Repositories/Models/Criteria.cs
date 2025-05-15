using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models
{
    public class Criteria
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public bool IsEvaluation { get; set; } = false;

        [MaxLength(255)]
        public string MailDomain { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        public string Description { get; set; }

        // Navigation property for the join table with Evaluation through CriteriaEvaluate
        public virtual ICollection<CriteriaEvaluate> CriteriaEvaluates { get; set; } = new List<CriteriaEvaluate>();
    }
}
