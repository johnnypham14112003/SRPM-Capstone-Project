using System;
using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models
{
    public class ResearchPaper
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string RefLink { get; set; } // Stores the reference link to the research paper

        [MaxLength(250)]
        public string Title { get; set; }

        public string Content { get; set; }

        [Required]
        public int Viewed { get; set; } = 0; // Default view count is 0

        [MaxLength(100)]
        public string ProviderName { get; set; }

        [Required]
        public DateTime AddedDate { get; set; } = DateTime.UtcNow;

        // Foreign Keys
        [Required]
        public Guid FieldId { get; set; }
        public Field Field { get; set; } // One Field -> Many Research Papers

        [Required]
        public Guid PrincipalInvestigatorId { get; set; }
        public Account PrincipalInvestigator { get; set; } // One Account -> Many Research Papers
    }
}
