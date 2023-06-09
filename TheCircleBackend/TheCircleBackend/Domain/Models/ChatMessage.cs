using TheCircleBackend.Domain.Interfaces;

namespace TheCircleBackend.Domain.Models
{
    public class ChatMessage: IDomain
    {
        public String Message { get; set; }
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
        public int StreamId { get; set; }

        public WebsiteUser Writer { get; set; }
        public Stream LiveStream { get; set; }

        //Private properties, Date cannot be removed
        private DateTime? _Date { get; set; }
    }
}
