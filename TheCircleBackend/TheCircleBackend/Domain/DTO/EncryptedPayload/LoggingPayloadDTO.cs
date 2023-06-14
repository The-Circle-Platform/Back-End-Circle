using TheCircleBackend.Domain.Interfaces;
using TheCircleBackend.Domain.Models;

namespace TheCircleBackend.Domain.DTO.EncryptedPayload
{
    public class LoggingPayloadDTO: IContent
    {
        public LogItem OriginalData { get; set; }
    }

    public class LoggingOutDTO : IOutComingContent { 
        public string OriginalText { get; set; } = string.Empty;
    }
}
