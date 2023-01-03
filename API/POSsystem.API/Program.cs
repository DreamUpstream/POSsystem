using POSsystem.Migrations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using POSsystem.Core.Services;

namespace POSsystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = CreateHostBuilder(args).Build();

            using var scope = builder.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            if (db.Database.GetPendingMigrations().Any())
            {
                db.Database.Migrate();
            }
            if (!db.Users.Any())
            {
                Task.Run(async () =>
                {
                    var salt = RandomNumberGenerator.GetBytes(128 / 8);
                    await db.Users.AddAsync(new Contracts.Data.Entities.User
                    {
                        EmailAddress = "admin@admin.com",
                        Salt = salt,
                        Password = PasswordHashingService.Hash("admin", salt),
                        Role = Contracts.Enum.UserRole.Owner,
                        
                    });
                    await db.SaveChangesAsync();
                });
            }

            builder.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
