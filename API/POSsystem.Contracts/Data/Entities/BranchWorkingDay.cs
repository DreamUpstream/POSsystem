using System.ComponentModel.DataAnnotations;
using POSsystem.Contracts.Enum;

namespace POSsystem.Contracts.Data.Entities;

public class BranchWorkingDay : AuditableEntity
{
    public WorkingDay WorkingDay { get; set; }
    public DateTime WorkingHoursStart { get; set; }
    public DateTime WorkingHoursEnd { get; set; }
    public int BranchId { get; set; }
}
