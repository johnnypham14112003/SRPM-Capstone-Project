using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Repositories.Models
{
    public class ContentTable
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // Index
        [Required]
        public int ColumnIndex { get; set; } = 1;

        [Required]
        public int RowIndex { get; set; } = 1;

        // Column Title
        [MaxLength(255)]
        public string? ColumnTitle { get; set; }

        [MaxLength(30)]
        public string? ColumnTitleAlign { get; set; }

        [MaxLength(50)]
        public string? ColumnTitleStyle { get; set; }

        // Sub-Column Title
        [MaxLength(255)]
        public string? SubColumnTitle { get; set; }

        [MaxLength(30)]
        public string? SubColumnTitleAlign { get; set; }

        [MaxLength(50)]
        public string? SubColumnTitleStyle { get; set; }

        // Cell
        public string? CellContent { get; set; }

        [MaxLength(30)]
        public string? CellContentAlign { get; set; }

        [MaxLength(50)]
        public string? CellContentStyle { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public Guid FieldContentId { get; set; }

        // Navigation properties
        public virtual FieldContent FieldContent { get; set; } = null!;
    }

}
