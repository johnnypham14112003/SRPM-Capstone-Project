using System;
using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models
{
    public class ProjectTag
    {
        [Key]
        public Guid Id { get; set; }

        [MaxLength(150)]
        public string Name { get; set; }

        // Foreign Key
        [Required]
        public Guid ProjectId { get; set; }
        public Project Project { get; set; } // One Project -> Many Tags
    }
}
