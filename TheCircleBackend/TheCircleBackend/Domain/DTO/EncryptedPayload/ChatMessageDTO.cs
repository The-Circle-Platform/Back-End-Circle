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
        public List<ChatMessageLite>? OriginalList { get; set; }
    }

    public class ChatMessageLite
    {
        public string? message { get; set; }
        public DateTime? date
        {
            get
            {
                return _date;
            }
            set
            {
                if (_date == null)
                {
                    this._date = value;
                }
                else
                {
                    throw new InvalidOperationException("Date cannot be changed");
                }
            }
        }

        //Foreign key relations
        public int webUserId { get; set; }
        public int receiverId { get; set; }

        public string? writer { get; set; }

        public string? receiverUser { get; set; }

        //Private properties, Date cannot be removed
        private DateTime? _date { get; set; }
    }


}
