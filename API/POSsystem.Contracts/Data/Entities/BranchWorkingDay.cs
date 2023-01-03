using System.ComponentModel.DataAnnotations.Schema;
using POSsystem.Contracts.Enum;

namespace POSsystem.Contracts.Data.Entities;

public class BranchWorkingDay : AuditableEntity
{
    public WorkingDay WorkingDay { get; set; }
    public DateTime WorkingHoursStart { get; set; }
    public DateTime WorkingHoursEnd { get; set; }
    [ForeignKey("branches")]
    public int BranchId { get; set; }
}
