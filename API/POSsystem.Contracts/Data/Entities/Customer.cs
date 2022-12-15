using POSsystem.Contracts.Enum;

namespace POSsystem.Contracts.Data.Entities;

public class Customer : User
{
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public DateTimeOffset RegisteredDate { get; set; }
    public CustomerStatus Status { get; set; }
    public Discount Discount { get; set; }
}