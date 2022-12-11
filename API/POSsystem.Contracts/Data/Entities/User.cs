using POSsystem.Contracts.Enum;

namespace POSsystem.Contracts.Data.Entities
{
    public class User : BaseEntity
    {
        public string EmailAddress { get; set; }
        public UserRole Role { get; set; }
        public string Password { get; set; }
    }
}