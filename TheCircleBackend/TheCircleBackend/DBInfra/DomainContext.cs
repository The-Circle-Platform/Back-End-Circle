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
                PrivateKey = "MIICeAIBADANBgkqhkiG9w0BAQEFAASCAmIwggJeAgEAAoGBAKHk10g1evWq9t7KEEu8sKUGGKwwjx+X7fsn0ELt71G0jtdo43jjF8x4Q2S1Ho1VGV3XKMagRYf4t/Z3z8THd2c0g4SFwVSvQca/+BeJ9qmuVji7mw5x67PeOUWzJS0240i1tJFXHbLfXAgg666GIrkI135ElhwJH9Alu6xQ2VetAgMBAAECgYB/GuojGUoGo0nbtQ2CSQzvI5AvcJiOF3yS2blbMu/YWEhlu0YM3U8MC8ftw33PPOcDlC/Bcofkr1PPwFVxi6GkOBxDPiHthzruGGlnzbSMA9Ldo9qf/9ZUzKay26fhEVQoACNGsvw4GZxAblJ3UkqBnPea1chGQWh9v2xlo4YRiQJBAMwwtMER3URwxoUZGfyG4tTVfg/TK7AF5Iz6c5YQDx/UoSTrKLIM14f+APdvhMCAL5G+AEDKhesrUNpTXvNND3cCQQDK+MBpmfb/oDnqxVtFoY+pW3d7EjUyNXixM62xSJk5IdAX0gqwiMveb3svAM6SF2ro0b/IGItfNSaVxxtIxcL7AkEAiFwGeeDqOShvCreGqSOTG7svInZNeJGW3abrxc0XrJQcwUDhvnXhAYpZLuSkbMGuAtA17w7QfApDRmniwOw3ZQJBAKZnnkh1pB0bXaBuwU+rDz8H8EMEQHyzfgm5lrN8E7LVV+fPmlf1Lz9kIpf8j18St+G85QDFrq4Vw1aUcHgPOrUCQQDGLgVLCaOe4vE+0RMfwyI1FO8kXfARz0whL/8TevtnvvghLP2ogUNUswhyaZUS5bA8AxeVrHxYre1GGQCyv8DM",
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
