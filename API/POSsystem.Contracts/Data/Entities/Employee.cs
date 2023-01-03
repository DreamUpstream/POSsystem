using System.ComponentModel.DataAnnotations.Schema;
using POSsystem.Contracts.Enum;

namespace POSsystem.Contracts.Data.Entities;

public class Employee : AuditableEntity
{
    public string Name { get; set; }
    [ForeignKey("users")]
    public int UserId { get; set; }
    public DateTime RegisteredDate { get; set; }
    public EmployeeStatus Status { get; set; }
    [ForeignKey("companies")]
    public int CompanyId { get; set; }
}