using POSsystem.Contracts.Data.Entities;
using POSsystem.Contracts.Enum;

namespace POSsystem.Contracts.DTO;

public class CustomerDTO : BaseEntity
{
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public CreateOrUpdateUserDTO User { get; set; }
    public DateTime RegisteredDate { get; set; }
    public CustomerStatus Status { get; set; }
    public int Discount { get; set; }
}