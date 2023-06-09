using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TheCircleBackend.Domain.AuthModels;

namespace TheCircleBackend.DBInfra
{
    public class IdentityDBContext : IdentityDbContext<IdentityUser>
    {
        public IdentityDBContext(DbContextOptions<IdentityDBContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
