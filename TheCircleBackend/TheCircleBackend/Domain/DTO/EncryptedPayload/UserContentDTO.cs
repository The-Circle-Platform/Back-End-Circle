using TheCircleBackend.Domain.Interfaces;
using TheCircleBackend.Domain.Models;

namespace TheCircleBackend.Domain.DTO.EncryptedPayload
{
    public class UserIncomingDTO : IContent
    {
        //One of these elements are being used in the signature.
        public LoginBody OriginalUserRequest { get; set; }
    }

    public class UserRequestBody
    {
        public int OwnUserId { get; set; }
        public int RequestUserId { get; set; }
    }

    public class UserContentDTO : IContent
    {
        public WebsiteUser? OriginalData { get; set; }
        public List<WebsiteUser>? OriginalList { get; set; }
        public string ServerPublicKey { get; set; }
    }

    public class LoginBody
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

}
