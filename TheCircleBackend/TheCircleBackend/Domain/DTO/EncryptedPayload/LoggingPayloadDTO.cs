using TheCircleBackend.Domain.Interfaces;
using TheCircleBackend.Domain.Models;

namespace TheCircleBackend.Domain.DTO.EncryptedPayload
{
    public class LoggingPayloadDTO: IContent
    {
        public LogItem OriginalData { get; set; }
    }

    public class LoggingOutDTO : IOutComingContent { 
        public LogItem OriginalData { get; set; }
    }

    public class LoggingOutTextDTO : IOutComingContent
    {
        public string OriginalData { get; set; }
    }

    public class LoggingIdDTO : IContent
    {
        public int OriginalData { get; set; }
    }
}
