using POSsystem.Contracts.Enum;

namespace POSsystem.Contracts.DTO;

public class CreateOrUpdateBranchDTO
{
    public string Address { get; set; }
    public string Contacts { get; set; }
    public BranchStatus BranchStatus { get; set; }
    public int CompanyId { get; set; } 
    public IEnumerable<CreateOrUpdateBranchWorkingDayDTO> BranchWorkingDays { get; set; }
}
public class CreateOrUpdateBranchWorkingDayDTO
{
    public int WorkingDay { get; set; }
    public DateTime WorkingHoursStart { get; set; }
    public DateTime WorkingHoursEnd { get; set; }
}

