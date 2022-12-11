using POSsystem.Contracts.Data.Repositories;

namespace POSsystem.Contracts.Data
{
    public interface IUnitOfWork
    {
        IItemRepository Items { get; }
        IUserRepository Users { get; }
        Task CommitAsync();
    }
}