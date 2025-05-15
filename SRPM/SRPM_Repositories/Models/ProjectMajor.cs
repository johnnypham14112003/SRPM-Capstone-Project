using System;
using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models
{
    public class ProjectMajor
    {
        [Required]
        public Guid ProjectId { get; set; }
        public Project Project { get; set; } // Many Projects -> Many Majors

        [Required]
        public Guid MajorId { get; set; }
        public Major Major { get; set; } // Many Majors -> Many Projects

        // Composite Primary Key
        [Key]
        public Guid Id { get; set; }
    }
}
