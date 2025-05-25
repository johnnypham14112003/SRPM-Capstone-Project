using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Repositories.Models
{
    public class FieldContent
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string? Title { get; set; }

        [MaxLength(30)]
        public string? TitleAlign { get; set; }

        [MaxLength(50)]
        public string? TitleStyle { get; set; }

        // Content
        public string? Content { get; set; }

        [MaxLength(30)]
        public string? ContentAlign { get; set; }

        [MaxLength(50)]
        public string? ContentStyle { get; set; }

        [Required]
        public int IndexInField { get; set; } = 1;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public Guid DocumentFieldId { get; set; }

        // Navigation properties
        public virtual DocumentField DocumentField { get; set; } = null!;
        public virtual ICollection<ContentTable> ContentTables { get; set; } = new List<ContentTable>();
    }
}
