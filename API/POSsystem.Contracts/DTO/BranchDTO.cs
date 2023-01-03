using POSsystem.Contracts.Data.Entities;
using POSsystem.Contracts.Enum;

namespace POSsystem.Contracts.DTO;

public class BranchDTO : AuditableEntity
{
    public string Address { get; set; }
    public string Contacts { get; set; }
    public BranchStatus Status { get; set; }
    public int CompanyId { get; set; } 
    public IEnumerable<CreateOrUpdateBranchWorkingDayDTO> BranchWorkingDays { get; set; }


}