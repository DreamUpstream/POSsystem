using POSsystem.Contracts.Data.Entities;
using POSsystem.Contracts.Data.Repositories;
using POSsystem.Migrations;

namespace POSsystem.Core.Data.Repositories;

public class BranchWorkingDaysRepository : Repository<BranchWorkingDays>, IBranchWorkingDaysRepository
{
    public BranchWorkingDaysRepository(DatabaseContext context) : base(context)
    {
        
    }
}