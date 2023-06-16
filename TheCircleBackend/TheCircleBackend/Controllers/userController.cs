using Microsoft.AspNetCore.Mvc;
using System.Net;
using TheCircleBackend.Domain.Models;
using TheCircleBackend.DomainServices.IRepo;
using Microsoft.Extensions.Logging;
using TheCircleBackend.Helper;
using System.Security.Claims;
using System.Linq.Expressions;

namespace TheCircleBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class userController : ControllerBase
    {

        private readonly IWebsiteUserRepo websiteUserRepo;
        private readonly ILogItemRepo logItemRepo;
        private readonly LogHelper logHelper;

        public userController(IWebsiteUserRepo websiteUserRepo, ILogItemRepo logItemRepo, ILogger<userController> logger)
        {
            this.websiteUserRepo = websiteUserRepo;
            this.logItemRepo = logItemRepo;
            this.logHelper = new LogHelper(logItemRepo, logger);

        }

        [HttpGet]
        public IEnumerable<WebsiteUser> Get()
        {
            return websiteUserRepo.GetAllWebsiteUsers();

        }

        [HttpPut("{id}/pfp")]
        public IActionResult postImage(WebsiteUser websiteUser, int id)
        {
            var user = this.websiteUserRepo.GetById(id);

            var ip = this.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            var endpoint = "POST /user";
            /* var currentUser = this.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
             var action = $"WebsiteUser with ID: {user.Id}, Name: {user.UserName}";
             logHelper.AddUserLog(ip, endpoint, currentUser, action);*/

            Console.WriteLine(user);
            try
            {
                user.Base64Image = websiteUser.Base64Image;
                this.websiteUserRepo.Update(user, id);
                return Ok("user updated");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e);
            }
        }

        [HttpGet("{id}")]
        public IActionResult get(int id)
        {
            Console.WriteLine(id);
            var user = websiteUserRepo.GetById(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);

        }

        [HttpPost]
        public IActionResult post(WebsiteUser user)
        {
            var ip = this.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            var endpoint = "POST /user";
            var currentUser = this.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var action = $"WebsiteUser with ID: {user.Id}, Name: {user.UserName}";
            logHelper.AddUserLog(ip, endpoint, currentUser, action);

            Console.WriteLine(user);
            try
            {
                this.websiteUserRepo.Add(user);
                return Ok("user added");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e);
            }
        }

        [HttpPut("{id}")]
        public IActionResult put(WebsiteUser user, int id)
        {
            var ip = this.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            var endpoint = $"PUT /user/{id}";
            var subjectUser = this.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var action = $"WebsiteUser with ID: {user.Id}, Name: {user.UserName}";
            logHelper.AddUserLog(ip, endpoint, subjectUser, action); Console.WriteLine(id);

            try
            {
                websiteUserRepo.Update(user, id);
                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e);
            }
        }
        [HttpGet("{id}/pfp")]
        public IActionResult getPfp(WebsiteUser pfp)
        {
            string pic = websiteUserRepo.GetById(pfp.Id).Base64Image;
            Console.WriteLine(pfp);
            var user = websiteUserRepo.GetById(pfp.Id).Base64Image;
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);

        }
    
        

    }
}
