using POSsystem.Contracts.Enum;

namespace POSsystem.Contracts.Data.Entities;

public class Branch : AuditableEntity
{
    public string Address { get; set; }
    public string Contacts { get; set; }
    public BranchStatus BranchStatus { get; set; }
    public int CompanyId { get; set; }
}