using POSsystem.Contracts.Data.Entities;

namespace POSsystem.Contracts.DTO;

public class BranchWorkingDayDTO : AuditableEntity
{
    public int WorkingDay { get; set; }
    public DateTime WorkingHoursStart { get; set; }
    public DateTime WorkingHoursEnd { get; set; }
}