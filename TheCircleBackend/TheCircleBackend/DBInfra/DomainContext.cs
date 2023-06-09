using Microsoft.EntityFrameworkCore;
using TheCircleBackend.Domain.Models;

namespace TheCircleBackend.DBInfra
{
    public class DomainContext : DbContext
    {
        public DbSet<WebsiteUser> WebsiteUser { get; set; } = null!;
        public DbSet<LogItem> LogItem { get; set; } = null!;
        public DbSet<ChatMessage> ChatMessage { get; set; } = null!;
        public DbSet<Viewer> Viewer { get; set; } = null!;

        public  DomainContext(DbContextOptions<DomainContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WebsiteUser>().HasKey(wu => wu.Id);
            //modelBuilder.Entity<WebsiteUser>().HasAlternateKey(wu => wu.UserName);
            modelBuilder.Entity<WebsiteUser>().HasIndex(u => u.UserName).IsUnique();

            //Chatmessage
            modelBuilder.Entity<ChatMessage>().HasKey(cm => cm.Id);
          
            modelBuilder.Entity<ChatMessage>()
                .HasOne(cm => cm.ReceiverUser)
                .WithMany(ls => ls.StreamChatMessages)
                .HasForeignKey(cm => cm.ReceiverId);

            modelBuilder.Entity<ChatMessage>()
                .HasOne(cm => cm.Writer)
                .WithMany(ls => ls.UserChatMessages)
                .HasForeignKey(cm => cm.StreamId);
>>>>>>>>> Temporary merge branch 2
        }
    }
}
