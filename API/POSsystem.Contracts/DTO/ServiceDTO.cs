using POSsystem.Contracts.Enum;

namespace POSsystem.Contracts.DTO;

public class ServiceDTO
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int Duration { get; set; }
    public ServiceType Type { get; set; }
    public ServiceStatus Status { get; set; }
    public int DiscountId { get; set; }
    public int BranchId { get; set; }
}