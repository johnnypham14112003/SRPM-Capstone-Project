using System;
using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models
{
    public class CriteriaEvaluate
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid CriteriaId { get; set; }

        [Required]
        public Guid EvaluationId { get; set; }

        [Range(0, 100)]
        public int Rating { get; set; } = 0;

        public string Comment { get; set; }

        // Navigation properties
        public virtual Criteria Criteria { get; set; }
        public virtual Evaluation Evaluation { get; set; }
    }
}
