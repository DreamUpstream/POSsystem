using POSsystem.Contracts.Enum;

namespace POSsystem.Contracts.DTO
{
    public class CreateOrUpdateUserDTO
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public int Status { get; set; }
        public UserRole Role { get; set; }
    }
}