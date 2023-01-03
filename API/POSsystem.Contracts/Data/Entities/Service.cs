using System.ComponentModel.DataAnnotations.Schema;
using POSsystem.Contracts.Enum;

namespace POSsystem.Contracts.Data.Entities;

public class Service : AuditableEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int Duration { get; set; }
    public ServiceType Type { get; set; }
    public ServiceStatus Status { get; set; }
    [ForeignKey("discounts")]
    public int DiscountId { get; set; }
    [ForeignKey("branches")]
    public int BranchId { get; set; }
}