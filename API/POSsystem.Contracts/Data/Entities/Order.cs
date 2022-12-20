using POSsystem.Contracts.Enum;

namespace POSsystem.Contracts.Data.Entities;

public class Order : AuditableEntity
{
    public DateTime SubmissionDate { get; set; }
    public DateTime FulfilmentDate { get; set; }
    public Decimal Tip { get; set; }
    public Boolean DeliveryRequired { get; set; }
    public string Comment { get; set; }
    public OrderStatus Status { get; set; }
    public Customer Customer { get; set; }
    public Employee Employee { get; set; }
    public Discount Discount { get; set; }
    public string Delivery { get; set; }
    public List<Item> Products { get; set; }
    public List<Service> Services { get; set; }
}