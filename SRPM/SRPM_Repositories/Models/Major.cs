using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models
{
    public class Major
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        // Navigation property: One Major -> Many Accounts
        public ICollection<Account> Accounts { get; set; } = new List<Account>();

        // Navigation property: Many-to-many relationship with Projects via the ProjectMajor join entity
        public ICollection<ProjectMajor> ProjectMajors { get; set; } = new List<ProjectMajor>();
    }
}
