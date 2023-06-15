using System.ComponentModel.DataAnnotations;
using TheCircleBackend.Domain.Interfaces;
using TheCircleBackend.Domain.Models;

namespace TheCircleBackend.Domain.DTO.EncryptedPayload
{
    public class ChatMessageDTOIncoming : IContent
    {
        public ChatMessage? Message { get; set; }
    }

    public class ChatMessageDTOOutcoming : IContent
    {
        public List<ChatMessage>? Messages { get; set; }
    }



}
