namespace TheCircleBackend.Domain.DTO
{
    public class VideoStreamDTO
    {
        public int id { get; set; }
        public string? title { get; set; }
        public DateTime startStream { get; set; }
        public DateTime? endStream { get; set; }
        public string? streamKey { get; set; }
        public string transparantUserName { get; set; }
        public int transparantUserId { get; set; }
    }
}
