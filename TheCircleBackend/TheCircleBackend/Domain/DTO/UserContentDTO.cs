using TheCircleBackend.Domain.Interfaces;
using TheCircleBackend.Domain.Models;

namespace TheCircleBackend.Domain.DTO
{
    public class UserIncomingDTO: IContent
    {
        //One of these elements are being used in the signature.
        public UserRequestBody OriginalUserRequest { get; set; }
    }

    public class UserRequestBody
    {
        public int OwnUserId { get; set; }
        public int RequestUserId { get; set; }
    }

    public class UserContentDTO : IContent
    {
        public UserBody OriginalData { get; set; }
        public string ServerPublicKey { get; set; }
    }

    public class UserBody { 
        public int OwnUserId { get; set; }
        public List<WebsiteUser>? OriginalUserList { get; set; }
        public WebsiteUser? OriginalUser { get; set; }
    }

}
