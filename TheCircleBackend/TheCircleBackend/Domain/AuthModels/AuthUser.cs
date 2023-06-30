using Microsoft.AspNetCore.Identity;

namespace TheCircleBackend.Domain.AuthModels
{
    public class AuthUser : IdentityUser
    {
        public string UserName { get; set; }



    }
}
