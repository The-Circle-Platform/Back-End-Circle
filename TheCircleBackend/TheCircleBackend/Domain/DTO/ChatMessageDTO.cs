using System.ComponentModel.DataAnnotations;
using TheCircleBackend.Domain.Interfaces;
using TheCircleBackend.Domain.Models;

namespace TheCircleBackend.Domain.DTO
{
    public abstract class IChatDTO {
        public int UserId { get; set; }
        public int ReceiverId { get; set; }
    }

    public class ChatMessageDTOIncoming: IChatDTO
    {
        public ChatMessage? Message { get; set; }
    }

    public class ChatMessageDTOOutcoming : IChatDTO
    {
        public List<ChatMessage>? Messages { get; set; }
    }


    
}
