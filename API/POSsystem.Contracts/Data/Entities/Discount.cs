using POSsystem.Contracts.Enum;

namespace POSsystem.Contracts.Data.Entities;

public class Discount : AuditableEntity
{
    public decimal Amount { get; set; }
    public DiscountMeasure Measure { get; set; }
    public string PromoCode { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}