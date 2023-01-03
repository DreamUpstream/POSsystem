using Microsoft.EntityFrameworkCore;
using POSsystem.Contracts.Data.Entities;
using POSsystem.Contracts.Enum;
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
        
        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            modelBuilder.Entity<Employee>().HasData(new Employee 
            {
                Id = 1,
                CreatedBy = "DbContext",
                Name = "Employee1",
                UserId = 1,
                RegisteredDate = DateTime.UtcNow,
                Status = EmployeeStatus.Active,
                CompanyId = 1
            });

            modelBuilder.Entity<Service>().HasData(new Service
            {
                Id = 1,
                CreatedBy = "DbContext",
                Name = "Service1",
                Description = "Service1",
                Price = 1,
                Duration = 1,
                Type = 0,
                Status = Contracts.Enum.ServiceStatus.Available,
                DiscountId = 1,
                BranchId = 1
            });

            modelBuilder.Entity<Order>().HasData(new Order
            {
                Id = 1,
                CreatedBy = "DbContext",
                SubmissionDate = DateTime.UtcNow,
                FulfilmentDate = DateTime.UtcNow,
                Tip = 1,
                DeliveryRequired = false,
                Comment = "",
                Status = OrderStatus.Created,
                CustomerId = 1,
                EmployeeId = 1,
                DiscountId = 1,
                Delivery = ""
            });

            modelBuilder.Entity<Discount>().HasData(new Discount
            {
                Id = 1,
                CreatedBy = "DbContext",
                Amount = 10,
                Measure = DiscountMeasure.Money,
                PromoCode = "PROMO42",
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.Add(TimeSpan.FromDays(30))
            });

            modelBuilder.Entity<Branch>().HasData(new Branch
            {
                Id = 1,
                Address = "Savanoriu pr. 1, Vilnius",
                Contacts = "maistas@admin.com",
                BranchStatus = BranchStatus.Closed,
                CompanyId = 1
            });

            modelBuilder.Entity<Company>().HasData(new Company
            {
                Id = 1,
                Name = "The food company inc."
            });

            modelBuilder.Entity<BranchWorkingDay>().HasData(new BranchWorkingDay
            {
                WorkingDay = WorkingDay.Friday,
                WorkingHoursStart = DateTime.Now,
                WorkingHoursEnd = DateTime.Now.Add(TimeSpan.FromHours(10)),
                BranchId = 1
            });
            
            modelBuilder.Entity<BranchWorkingDay>().HasData(new BranchWorkingDay
            {
                WorkingDay = WorkingDay.Monday,
                WorkingHoursStart = DateTime.Now,
                WorkingHoursEnd = DateTime.Now.Add(TimeSpan.FromHours(10)),
                BranchId = 1
            });
            
            modelBuilder.Entity<BranchWorkingDay>().HasData(new BranchWorkingDay
            {
                WorkingDay = WorkingDay.Wednesday,
                WorkingHoursStart = DateTime.Now,
                WorkingHoursEnd = DateTime.Now.Add(TimeSpan.FromHours(10)),
                BranchId = 1
            });

            modelBuilder.Entity<Customer>().HasData(new Customer
            {
                Id = 1,
                Name = "Jane",
                PhoneNumber = "1111111111",
                UserId = 1,
                RegisteredDate = DateTime.Now,
                Status = CustomerStatus.Active,
                DiscountId = 1
            });

            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                EmailAddress = "email@trustable_email_service.ll",
                Role = UserRole.Base,
                Password = "password",
            });

            modelBuilder.Entity<Service>().HasData(new Service()
            {
                Name = "Service",
                Description = "Descripotion",
                Price = new decimal(20.99),
                Duration = 1,
                Type = ServiceType.Default,
                Status = ServiceStatus.Available,
                DiscountId = 1,
                BranchId = 1
            });

            modelBuilder.Entity<ServiceReservation>().HasData(new ServiceReservation
            {
                Time = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"),
                ReservationStatus = ReservationStatus.Registered,
                ServiceId = 1,
                TaxId = 22,
                OrderId = 1,
                EmployeeId = 1
            });

            modelBuilder.Entity<Item>().HasData(new Item
            {
                Price = new decimal(9.99),
                Name = "name",
                Description = "Description",
                Status = ItemStatus.Available,
                CategoryId = 1,
                DiscountId = 1,
                ColorCode = "#ff0011"
            });

            modelBuilder.Entity<ItemCategory>().HasData(new ItemCategory
            {
                Name = "Category 1"
            });
        }
    }
}