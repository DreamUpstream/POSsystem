using POSsystem.Contracts.Data.Entities;
using POSsystem.Contracts.Enum;

namespace POSsystem.Contracts.DTO
{
    public class OrderDTO
    {
        public DateTime SubmissionDate { get ; set; }
        public Decimal Tip { get ; set; }
        public Boolean DeliveryRequired { get; set; }
        public string? Comment { get; set; }
        public OrderStatus Status { get; set; }
        public int CustomerId { get; set; }
        public int EmployeeId { get; set; }
        public int? DiscountId { get; set; }
        public string Delivery { get; set; }
        public List<Item> Products { get; set; }
        public List<Service> Services { get; set; }
    }
}