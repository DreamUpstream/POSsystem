using POSsystem.Contracts.Data.Entities;
using POSsystem.Contracts.Data.Repositories;
using POSsystem.Migrations;

namespace POSsystem.Core.Data.Repositories;

public class PurchasableItemRepository : Repository<PurchasableItem>, IPurchasableItemRepository
{
    public PurchasableItemRepository(DatabaseContext context) : base(context)
    {
        
    }
}