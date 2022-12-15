using POSsystem.Contracts.Data.Entities;
using POSsystem.Contracts.Data.Repositories;
using POSsystem.Migrations;

namespace POSsystem.Core.Data.Repositories;

public class BranchRepository : Repository<Branch>, IBranchRepository
{
    public BranchRepository(DatabaseContext context) : base(context)
    {
        
    }
}