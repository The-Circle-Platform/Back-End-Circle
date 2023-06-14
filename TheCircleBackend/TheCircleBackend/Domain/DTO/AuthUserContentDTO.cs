using TheCircleBackend.Domain.Interfaces;

namespace TheCircleBackend.Domain.DTO
{
    public class AuthUserContentDTO: IContent
    {
        public LoginDTO OriginalLoginData { get; set; }
    }
}
