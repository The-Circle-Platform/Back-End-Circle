using System.ComponentModel.DataAnnotations;
using TheCircleBackend.Domain.Interfaces;
using TheCircleBackend.Domain.Models;

namespace TheCircleBackend.Domain.DTO.EncryptedPayload
{
    public class ChatMessageDTOIncoming : IContent
    {
        public ChatMessage? OriginalData { get; set; }
    }

    public class ChatMessageDTOOutcoming : IOutComingContent
    {
        public ChatMessage? OriginalData { get; set; }
    }

    public class ChatListDTOOutcoming : IOutComingContent
    {
        public List<ChatMessage>? OriginalList { get; set; }
    }



}
