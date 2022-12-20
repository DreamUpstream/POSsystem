using POSsystem.Contracts.Enum;

namespace POSsystem.Contracts.Data.Entities;

public class Customer : BaseEntity
{
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public User User { get; set; }
    public DateTimeOffset RegisteredDate { get; set; }
    public CustomerStatus Status { get; set; }
    public Discount Discount { get; set; }
}