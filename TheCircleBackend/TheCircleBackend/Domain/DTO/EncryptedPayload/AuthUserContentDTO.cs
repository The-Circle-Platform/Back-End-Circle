using TheCircleBackend.Domain.Interfaces;

namespace TheCircleBackend.Domain.DTO.EncryptedPayload
{
    public class AuthUserContentDTO : IContent
    {
        public LoginDTO OriginalLoginData { get; set; }
    }
}
