using POSsystem.Contracts.Data.Entities;
using POSsystem.Contracts.Data.Repositories;
using POSsystem.Migrations;

namespace POSsystem.Core.Data.Repositories;

public class RoleRepository : Repository<Role>, IRoleRepository
{
    public RoleRepository(DatabaseContext context) : base(context)
    {
        
    }
}