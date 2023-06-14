using TheCircleBackend.Domain.Interfaces;
using TheCircleBackend.Domain.Models;

namespace TheCircleBackend.Domain.DTO.EncryptedPayload
{
    public class UserIncomingDTO : IContent
    {
        //One of these elements are being used in the signature.
        public UserRequest OriginalUserRequest { get; set; }
    }

    public class UserContentDTO : IOutComingContent
    {
        public WebsiteUser? OriginalData { get; set; }
        public List<WebsiteUser>? OriginalList { get; set; }
    }

    public class UserRequest
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
    }

}
