namespace TheCircleBackend.Domain.Models
{
    public class Viewer
    {
        public string? ConnectionId { get; set; }
        public int StreamId { get; set; }
        public int UserId { get; set; }

        public Stream? Stream { get; set; }
        public WebsiteUser? WebsiteUser { get; set; }
    }
}
