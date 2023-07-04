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
        public DbSet<KeyStore> UserKeys { get; set; } = null!;
        public DbSet<Domain.Models.Stream> VideoStream { get; set; } = null!;
        public DbSet<Streamchunks> Streamchunks { get; set; } = null!;
        public DbSet<Vod> Vod { get; set; } = null!;

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
                .HasForeignKey(cm => cm.WebUserId);

            //Watchers
            modelBuilder.Entity<Viewer>().HasKey(f => f.ConnectionId);
            modelBuilder.Entity<Viewer>()
                .HasOne(v => v.WebsiteUser)
                .WithMany(u => u.CurrentWatchList)
                .HasForeignKey(v => v.UserId);

            modelBuilder.Entity<Viewer>()
                .HasOne(v => v.Stream)
                .WithMany(s => s.ViewList)
                .HasForeignKey(v => v.StreamId);
            
            modelBuilder.Entity<KeyStore>().HasKey(ks => ks.Id);
            modelBuilder.Entity<KeyStore>().HasAlternateKey(ks => ks.UserId);

            // Model LogItem
            modelBuilder.Entity<LogItem>().HasKey(logItem => new { logItem.Id });

            //Model videoStream
            modelBuilder.Entity<Domain.Models.Stream>().HasKey(s => s.Id);
            modelBuilder.Entity<Domain.Models.Stream>().HasOne(tp => tp.User).WithMany(vs => vs.StreamList)
                .HasForeignKey(tp => tp.StreamUserId);

            modelBuilder.Entity<Streamchunks>().HasKey(sc => sc.Id);
            modelBuilder.Entity<Streamchunks>()
                .HasOne(sc => sc.RelatedStream)
                .WithMany(ss => ss.RelatedStreamChunks)
                .HasForeignKey(sc => sc.StreamId);

            modelBuilder.Entity<Vod>().HasKey(v => v.Id);

            WebsiteUser AdminUser = new()
            {
                Id = 1,
                IsOnline = false,
                UserName = "Admin",
            };

            KeyStore AdminKeys = new()
            {
                Id = 1,
                PublicKey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQC2v9CGMJwkju9GxJtZ2RP2TUJwmLLy1h4g6TAk+ilxEqNbV2Dzikh7n/pRJze/4vxj3J9q4547CBKrGxFJCP+IY2e32QmSMq5cgePHyv4jzheSixNe0oyqEy9AVaFzcxm1l+vCfSxNYpDiVElEmHyZFDgDVU+dYc85rNGQTXzxPQIDAQAB",
                UserId = 1
            };
            var UserList = new List<WebsiteUser>() { AdminUser };
            var KeyList = new List<KeyStore>() { AdminKeys };

            //Seeds admin data.
            modelBuilder.Entity<WebsiteUser>().HasData(UserList);
            modelBuilder.Entity<KeyStore>().HasData(KeyList);
        }
    }
}
