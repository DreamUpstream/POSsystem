using System.ComponentModel.DataAnnotations;
using POSsystem.Contracts.Enum;

namespace POSsystem.Contracts.Data.Entities;

public class BranchWorkingDays : AuditableEntity
{
    public WorkingDay WorkingDay { get; set; }
}