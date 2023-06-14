using TheCircleBackend.Domain.Interfaces;

namespace TheCircleBackend.Domain.DTO.EncryptedPayload
{
    public class AuthUserContentDTO : IContent
    {
        public LoginDTO OriginalLoginData { get; set; }
    }

    public class AuthOutRegisterDTO : IOutComingContent
    {
        public object OriginalLoad { get; set; }
    }

    public class AuthRegisterDTO : IContent
    {
        public RegisterDTO OriginalRegisterData { get; set; }
    }
}
