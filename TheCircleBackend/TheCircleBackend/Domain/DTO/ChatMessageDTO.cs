namespace TheCircleBackend.Domain.DTO
{
    public class ChatMessageDTO
    {
        public string? Uuid { get; set; }
        public int UserId { get; set; }
        public byte[]? Hash { get; set; }
        public byte[]? Signature { get; set; }
    }
}
