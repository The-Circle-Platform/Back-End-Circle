using Microsoft.EntityFrameworkCore;
using TheCircleBackend.Domain.Models;

namespace TheCircleBackend.DBInfra
{
    public class DomainContext : DbContext
    {
        public DbSet<WebsiteUser> WebsiteUser { get; set; } = null!;

        public  DomainContext(DbContextOptions<DomainContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WebsiteUser>().HasKey(wu => wu.Id);
            //modelBuilder.Entity<WebsiteUser>().HasAlternateKey(wu => wu.UserName);
            modelBuilder.Entity<WebsiteUser>().HasIndex(u => u.UserName).IsUnique();
        }
    }
}
