using Microsoft.EntityFrameworkCore;
using POSsystem.Contracts.Data.Entities;
using POSsystem.Contracts.Services;

namespace POSsystem.Migrations
{
    public class DatabaseContext : DbContext
    {
        private readonly IUserService _user;

        public DatabaseContext(DbContextOptions<DatabaseContext> options, IUserService user) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            _user = user;
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var item in ChangeTracker.Entries<User>().AsEnumerable())
            {
                if (item.State == EntityState.Added)
                {
                    item.Entity.Created = DateTime.UtcNow;
                }
            }

            foreach (var item in ChangeTracker.Entries<AuditableEntity>().AsEnumerable())
            {
                if (item.State == EntityState.Added)
                {
                    item.Entity.Created = DateTime.UtcNow;
                    item.Entity.CreatedBy = _user.UserId;
                }
                else if (item.State == EntityState.Modified)
                {
                    item.Entity.LastModified = DateTime.UtcNow;
                    item.Entity.ModifiedBy = _user.UserId;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        public DbSet<Branch> Branches { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ServiceReservation> ServiceReservations { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<BranchWorkingDay> BranchWorkingDays { get; set; }
    }
}