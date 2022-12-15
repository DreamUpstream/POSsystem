﻿using POSsystem.Contracts.Data.Repositories;

namespace POSsystem.Contracts.Data
{
    public interface IUnitOfWork
    {
        IBranchRepository Branches { get; }
        ICompanyRepository Companies { get; }
        ICustomerRepository Customers { get; }
        IDiscountRepository Discounts { get; }
        IEmployeeRepository Employees { get; }
        IItemCategoryRepository ItemCategories { get; }
        IItemRepository Items { get; }
        IOrderRepository Orders { get; }
        IPurchasableItemRepository PurchasableItems { get; }
        IServiceRepository Services { get; }
        IServiceReservationRepository ServiceReservations { get; }
        IUserRepository Users { get; }
        Task CommitAsync();
    }
}