using POSsystem.Contracts.Enum;

namespace POSsystem.Contracts.Data.Entities;

public class Employee : BaseEntity
{
    public string Name { get; set; }
    public User User { get; set; }
    public DateTimeOffset RegisteredDate { get; set; }
    public EmployeeStatus Status { get; set; }
    public UserRole UserRole { get; set; }
    public Company Company { get; set; }
}