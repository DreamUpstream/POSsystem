using POSsystem.Contracts.Data.Entities;

namespace POSsystem.Contracts.DTO
{
    public class ItemDTO : AuditableEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public long CategoryId { get; set; }
        public string ColorCode { get; set; }
    }
}