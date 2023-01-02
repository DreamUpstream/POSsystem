using POSsystem.Contracts.Enum;

namespace POSsystem.Contracts.DTO
{
    public class DiscountDTO
    {
        public decimal Amount { get; set; }
        public DiscountMeasure Measure { get; set; }
        public string PromoCode { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; } 
    }
}