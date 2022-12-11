using POSsystem.Contracts.Data.Entities;
using POSsystem.Contracts.Data.Repositories;
using POSsystem.Migrations;

namespace POSsystem.Core.Data.Repositories
{
    public class ItemRepository : Repository<Item>, IItemRepository
    {
        public ItemRepository(DatabaseContext context) : base(context)
        {
        }
    }
}