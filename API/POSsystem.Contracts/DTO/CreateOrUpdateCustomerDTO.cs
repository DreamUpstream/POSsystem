using POSsystem.Contracts.Enum;

namespace POSsystem.Contracts.DTO;

public class CreateOrUpdateCustomerDTO
{
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public CreateOrUpdateUserDTO User { get; set; }
    
    public CustomerStatus Status { get; set; }
    public int DiscountId { get; set; }
}