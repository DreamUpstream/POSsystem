using POSsystem.Contracts.Data;
using POSsystem.Contracts.Data.Repositories;
using POSsystem.Core.Data.Repositories;
using POSsystem.Migrations;

namespace POSsystem.Core.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DatabaseContext _context;

        public UnitOfWork(DatabaseContext context)
        {
            _context = context;
        }
        public IItemRepository Items => new ItemRepository(_context);

        public IUserRepository Users => new UserRepository(_context);

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}