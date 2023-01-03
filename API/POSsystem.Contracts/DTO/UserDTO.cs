using POSsystem.Contracts.Data.Entities;
using POSsystem.Contracts.Enum;

namespace POSsystem.Contracts.DTO;

public class UserDTO : AuditableEntity
{
    public string EmailAddress { get; set; }
    public UserRole Role { get; set; }
    public string Password { get; set; }
    public byte[] Salt { get; set; }
}
