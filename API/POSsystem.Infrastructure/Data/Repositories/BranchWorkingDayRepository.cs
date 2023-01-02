using POSsystem.Contracts.Data.Entities;
using POSsystem.Contracts.Data.Repositories;
using POSsystem.Migrations;

namespace POSsystem.Core.Data.Repositories;

public class BranchWorkingDayRepository : Repository<BranchWorkingDay>, IBranchWorkingDayRepository
{
    public BranchWorkingDayRepository(DatabaseContext context) : base(context)
    {
        
    }
}