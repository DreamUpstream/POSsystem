using System.ComponentModel.DataAnnotations.Schema;
using POSsystem.Contracts.Enum;

namespace POSsystem.Contracts.Data.Entities;

public class Branch : AuditableEntity
{
    public string Address { get; set; }
    public DateTime WorkingHoursStart { get; set; }
    public DateTime WorkingHoursEnd { get; set; }
    public BranchWorkingDays BranchWorkingDays { get; set; }
    public string Contacts { get; set; }
    public BranchStatus BranchStatus { get; set; }
    [ForeignKey("companies")]
    public int CompanyId { get; set; }
}