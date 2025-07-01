namespace SRPM_Services.BusinessModels.RequestModels;

public class RQ_TableStructure
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public int TableOrder { get; set; } = 1;
    public string ColumnJson { get; set; } = "[]";//["Mã","Tên","Số lượng"], ["001", "Sản phẩm A", "10"]

    // Foreign keys
    public Guid DocumentSectionId { get; set; }

    // Navigation properties
    public virtual ICollection<RQ_TableRow>? TableRows { get; set; }
}