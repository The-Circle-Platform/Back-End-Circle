using System.ComponentModel.DataAnnotations;
using TheCircleBackend.Domain.Interfaces;

namespace TheCircleBackend.Domain.DTO.EncryptedPayload
{
    public class IncomingChatContent : IContent
    {
        [Required]
        public ChatMessageDTOIncoming? OriginalContent { get; set; }
    }

    public class OutComingChatContent : IOutComingContent
    {
        [Required]
        public ChatMessageDTOOutcoming? OriginalContent { get; set; }
    }
}
