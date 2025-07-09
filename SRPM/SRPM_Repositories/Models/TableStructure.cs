using System.ComponentModel.DataAnnotations;

namespace SRPM_Repositories.Models;

public class TableStructure
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();

    [Required] public int TableOrder { get; set; } = 1;
    public string ColumnJson { get; set; } = "[]";//["Mã","Tên","Số lượng"], ["001", "Sản phẩm A", "10"]

    // Foreign keys
    [Required] public Guid DocumentSectionId { get; set; }

    // Navigation properties
    [Required] public virtual DocumentSection DocumentSection { get; set; } = null!;
    public virtual ICollection<TableRow>? TableRows { get; set; }
}