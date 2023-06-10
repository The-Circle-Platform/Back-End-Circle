using Microsoft.AspNetCore.Mvc;
using System.Net;
using TheCircleBackend.Domain.Models;
using TheCircleBackend.DomainServices.IRepo;
using Microsoft.Extensions.Logging;
using TheCircleBackend.Helper;

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
            this.logHelper = new LogHelper(logItemRepo, logger, "UserController");

        }

        [HttpGet]
        public IEnumerable<WebsiteUser> Get()
        {
            return websiteUserRepo.GetAllWebsiteUsers();

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
            Console.WriteLine(this.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString());
            logHelper.UserLog(this.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), "User " + 1 + " POST WebsiteUser with ID: " + user.Id + ", Name: " + user.UserName);
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
            logHelper.UserLog(Request.HttpContext.Connection.RemoteIpAddress.ToString(), "User " + 1 + " PUT WebsiteUser with ID: " + user.Id + ", Name: " + user.UserName);
            Console.WriteLine(id);
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
    }
}
