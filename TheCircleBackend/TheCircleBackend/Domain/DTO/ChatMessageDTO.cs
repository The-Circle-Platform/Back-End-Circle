using TheCircleBackend.Domain.Models;

namespace TheCircleBackend.Domain.DTO
{
    public abstract class IChatDTO {
        public int UserId { get; set; }
        public int ReceiverId { get; set; }
        public byte[] Signature { get; set; }
    }

    public class ChatMessageDTOIncomming: IChatDTO
    {
        public ChatMessage Messages { get; set; }
    }

    public class ChatMessageDTOOutcoming : IChatDTO
    {
        public ChatMessage[] Messages { get; set; }
        public string? ServerPublicKey { get; set; }
    }
}
