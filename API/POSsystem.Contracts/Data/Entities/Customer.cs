using System.ComponentModel.DataAnnotations.Schema;
using POSsystem.Contracts.Enum;

namespace POSsystem.Contracts.Data.Entities;

public class Customer : AuditableEntity
{
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    [ForeignKey("users")]
    public int UserId { get; set; }
    public DateTime RegisteredDate { get; set; }
    public CustomerStatus Status { get; set; }
    [ForeignKey("discounts")]
    public int Discount { get; set; }
}