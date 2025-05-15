using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models
{
    public class Field
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        // Navigation property: One Field -> Many Accounts
        public ICollection<Account> Accounts { get; set; } = new List<Account>();

        // Navigation property: One Field -> Many ResearchPapers
        public ICollection<ResearchPaper> ResearchPapers { get; set; } = new List<ResearchPaper>();

        // Navigation property: Many-to-many relationship with Projects via the ProjectField join entity
        public ICollection<ProjectField> ProjectFields { get; set; } = new List<ProjectField>();
    }
}
