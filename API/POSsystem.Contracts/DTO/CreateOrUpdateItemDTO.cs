namespace POSsystem.Contracts.DTO
{
    public class CreateOrUpdateItemDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public long CategoryId { get; set; }
        public string ColorCode { get; set; }
    }
}