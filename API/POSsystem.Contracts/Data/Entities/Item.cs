namespace POSsystem.Contracts.Data.Entities
{
    public class Item : AuditableEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        // Perhaps one item could have only one category?
        public ItemCategory Category { get; set; }
        public string ColorCode { get; set; }
    }
}