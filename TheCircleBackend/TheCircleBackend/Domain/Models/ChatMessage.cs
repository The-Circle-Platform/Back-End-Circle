using System.Text.Json.Serialization;
using TheCircleBackend.Domain.Interfaces;

namespace TheCircleBackend.Domain.Models
{
    public class ChatMessage: IDomain
    {
        public string? Message { get; set; }
        public DateTime? Date 
        { 
            get { 
                return _Date; 
            } 
            set 
            { 
                if(_Date == null)
                {
                    this._Date = value;
                }
                else
                {
                    throw new InvalidOperationException("Date cannot be changed");
                }
            } 
        }
        
        //Foreign key relations
        public int WebUserId { get; set; }
        public int ReceiverId { get; set; }
        
        public WebsiteUser? Writer { get; set; }

        public WebsiteUser? ReceiverUser { get; set; }

        //Private properties, Date cannot be removed
        private DateTime? _Date { get; set; }
    }
}
