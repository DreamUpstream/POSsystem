using POSsystem.Contracts.Enum;

namespace POSsystem.Contracts.Data.Entities;

public class PurchasableItem : AuditableEntity
{
    public decimal Price { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public ItemStatus Status { get; set; }
    public ItemCategory Category { get; set; }
    public Discount Discount { get; set; }
}