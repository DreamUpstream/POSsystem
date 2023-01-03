using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using POSsystem.Contracts.Enum;

namespace POSsystem.Contracts.Data.Entities;

public class Order : AuditableEntity
{
    public DateTime SubmissionDate { get; set; }
    public DateTime FulfilmentDate { get; set; }
    public Decimal Tip { get; set; }
    public Boolean DeliveryRequired { get; set; }
    [AllowNull]
    public string Comment { get; set; }
    public OrderStatus Status { get; set; }
    [ForeignKey("customers")]
    public int CustomerId { get; set; }
    [ForeignKey("employees")]
    public int EmployeeId { get; set; }
    [ForeignKey("discounts")]
    public int DiscountId { get; set; }
    [AllowNull]
    public string Delivery { get; set; }
    public List<Item> Products { get; set; }
    public List<Service> Services { get; set; }
}