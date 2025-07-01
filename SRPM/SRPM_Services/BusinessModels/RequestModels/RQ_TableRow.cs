using System.ComponentModel.DataAnnotations;

namespace SRPM_Services.BusinessModels.RequestModels;

public class RQ_TableRow
{
    public Guid? Id { get; set; }

    public int RowOrder { get; set; } = 1;
    public string CellsJson { get; set; } = "[]";

    // Foreign keys
    public Guid TableStructureId { get; set; }
}