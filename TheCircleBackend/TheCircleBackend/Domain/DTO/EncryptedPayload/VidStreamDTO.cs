using TheCircleBackend.Domain.Interfaces;

namespace TheCircleBackend.Domain.DTO.EncryptedPayload
{

    public class VidStreamDTO : IContent
    {
        public VideoStreamDTO OriginalData { get; set; }
    }
}
