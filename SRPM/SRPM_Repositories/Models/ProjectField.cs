using System;
using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models
{
    public class ProjectField
    {
        [Required]
        public Guid ProjectId { get; set; }
        public Project Project { get; set; } // Many Projects -> Many Fields

        [Required]
        public Guid FieldId { get; set; }
        public Field Field { get; set; } // Many Fields -> Many Projects

        // Composite Primary Key
        [Key]
        public Guid Id { get; set; }
    }
}
