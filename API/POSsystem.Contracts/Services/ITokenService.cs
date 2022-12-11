using POSsystem.Contracts.Data.Entities;
using POSsystem.Contracts.DTO;

namespace POSsystem.Contracts.Services
{
    public interface ITokenService
    {
        AuthTokenDTO Generate(User user);
    }
}