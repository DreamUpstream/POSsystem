using POSsystem.Contracts.Enum;

namespace POSsystem.Contracts.Data.Entities;

public class Employee : User
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public DateTimeOffset RegisteredDate { get; set; }
    public EmployeeStatus Status { get; set; }
    public Role Role { get; set; }
    public Company Company { get; set; }
}