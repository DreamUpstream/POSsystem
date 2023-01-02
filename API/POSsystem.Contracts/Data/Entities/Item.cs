using System.ComponentModel.DataAnnotations.Schema;
using POSsystem.Contracts.Enum;

namespace POSsystem.Contracts.Data.Entities
{
    public class Item : AuditableEntity
    {
        public decimal Price { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ItemStatus Status { get; set; }
        [ForeignKey("item_categories")]
        public int CategoryId { get; set; }
        [ForeignKey("discounts")]
        public int DiscountId { get; set; }
        public string ColorCode { get; set; }
        public Order? Order { get; set; }
    }
}