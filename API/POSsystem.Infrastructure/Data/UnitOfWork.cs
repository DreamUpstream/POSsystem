using POSsystem.Contracts.Data;
using POSsystem.Contracts.Data.Entities;
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

        public IBranchRepository Branches => new BranchRepository(_context);

        public IBranchWorkingDaysRepository WorkingDays => new BranchWorkingDaysRepository(_context);
        
        public ICompanyRepository Companies => new CompanyRepository(_context);
        
        public ICustomerRepository Customers => new CustomerRepository(_context);
        
        public IDiscountRepository Discounts => new DiscountRepository(_context);
        
        public IEmployeeRepository Employees => new EmployeeRepository(_context);
        
        public IItemCategoryRepository ItemCategories => new ItemCategoryRepository(_context);
        
        public IItemRepository Items => new ItemRepository(_context);

        public IOrderRepository Orders => new OrderRepository(_context);

        public IPurchasableItemRepository PurchasableItems => new PurchasableItemRepository(_context);

        public IRoleRepository Roles => new RoleRepository(_context);
        
        public IServiceRepository Services => new ServiceRepository(_context);

        public IServiceReservationRepository ServiceReservations => new ServiceReservationRepository(_context);
        
        public IUserRepository Users => new UserRepository(_context);

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}