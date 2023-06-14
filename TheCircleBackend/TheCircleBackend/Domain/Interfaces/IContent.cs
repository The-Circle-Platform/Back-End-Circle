namespace TheCircleBackend.Domain.Interfaces
{
    public abstract class IContent
    {
        public string? RandomId { get; set; } = new Guid().ToString();
        public byte[]? Signature { get; set; }
        public int SenderUserId { get; set; }
    }
}
