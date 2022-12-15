using POSsystem.Contracts.Enum;

namespace POSsystem.Contracts.Data.Entities;

public class Branch : AuditableEntity
{
    public string Address { get; set; }
    public TimeSpan WorkingHoursStart { get; set; }
    public TimeSpan WorkingHoursEnd { get; set; }
    public BranchWorkingDays BranchWorkingDays { get; set; }
    public string Contacts { get; set; }
    public BranchStatus BranchStatus { get; set; }
    public Company Company { get; set; }
}