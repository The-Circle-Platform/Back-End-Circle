using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PasswordGenerator;
using System.Data;
using TheCircleBackend.Domain.AuthModels;

namespace TheCircleBackend.DBInfra
{
    public class IdentityDBContext : IdentityDbContext<IdentityUser>
    {
        public static string Password = "Min DaMin2@?";
        public static string Role = "Admin";

        public IdentityDBContext(DbContextOptions<IdentityDBContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            IdentityUser Admin = new IdentityUser()
            {
                Id = "b74ddd14-6340-4840-95c2-db12554843e5",
                Email = "Admin@example.com",
                UserName = "Admin",
                NormalizedUserName = "ADMIN",
                NormalizedEmail = "ADMIN@EXAMPLE.COM",
                EmailConfirmed = true,
                LockoutEnabled = false
            };

            PasswordHasher<IdentityUser> passwordHasher = new PasswordHasher<IdentityUser>();
            Admin.PasswordHash = passwordHasher.HashPassword(Admin, Password);

            builder.Entity<IdentityUser>().HasData(Admin);

            base.OnModelCreating(builder);

            this.SeedUserRoles(builder);
            this.SeedRoles(builder);
        }

        private void SeedRoles(ModelBuilder builder)
        {
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole() { Id = "fab4fac1-c546-41de-aebc-a14da6895711", Name = "Admin", ConcurrencyStamp = "1", NormalizedName = "Admin" },
                new IdentityRole() { Id = "c7b013f0-5201-4317-abd8-c211f91b7330", Name = "User", ConcurrencyStamp = "2", NormalizedName = "User" }
                );
        }

        private void SeedUserRoles(ModelBuilder builder)
        {
            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>() { RoleId = "fab4fac1-c546-41de-aebc-a14da6895711", UserId = "b74ddd14-6340-4840-95c2-db12554843e5" }
            );
        }
    }
}
