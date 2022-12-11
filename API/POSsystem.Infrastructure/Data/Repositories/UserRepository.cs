using POSsystem.Contracts.Data.Entities;
using POSsystem.Contracts.Data.Repositories;
using POSsystem.Migrations;

namespace POSsystem.Core.Data.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(DatabaseContext context) : base(context)
        {
        }
    }
}