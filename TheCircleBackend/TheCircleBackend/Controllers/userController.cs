using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TheCircleBackend.Domain;

namespace TheCircleBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class userController : ControllerBase
    {

        [HttpGet]
        public IEnumerable<WebsiteUser> Get()
        {
            List<WebsiteUser> users = new List<WebsiteUser>();

            WebsiteUser user1 = new WebsiteUser();
            user1.Id = 0;
            user1.IsOnline = true;
            user1.UserName = "Henk";

            WebsiteUser user2 = new WebsiteUser();
            user2.Id = 1;
            user2.IsOnline = false;
            user2.UserName = "Ingrid";

            WebsiteUser user3 = new WebsiteUser();
            user3.Id = 2;
            user3.IsOnline = true;
            user3.UserName = "@realDonaldTrump";

            users.Add(user1);
            users.Add(user2);
            users.Add(user3);
            return users;



        }
    }
}
