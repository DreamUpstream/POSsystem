using POSsystem.Contracts.Enum;

namespace POSsystem.Contracts.Data.Entities;

public class Customer : BaseEntity
{
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public int UserId { get; set; }
    public DateTime RegisteredDate { get; set; }
    public CustomerStatus Status { get; set; }
    public int DiscountId { get; set; }
}