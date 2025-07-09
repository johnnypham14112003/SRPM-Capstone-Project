using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models;

public class TableRow
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();

    public int RowOrder { get; set; } = 1;
    public string CellsJson { get; set; } = "[]";

    // Foreign keys
    [Required] public Guid TableStructureId { get; set; }

    // Navigation properties
    public virtual TableStructure TableStructure { get; set; } = null!;
}