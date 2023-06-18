using Microsoft.AspNetCore.Mvc;
using System.Net;
using TheCircleBackend.Domain.Models;
using TheCircleBackend.DomainServices.IRepo;
using Microsoft.Extensions.Logging;
using TheCircleBackend.Helper;
using System.Security.Claims;

namespace TheCircleBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IWebsiteUserRepo websiteUserRepo;
        private readonly LogHelper logHelper;

        public UserController(IWebsiteUserRepo websiteUserRepo, ILogItemRepo logItemRepo, ILogger<UserController> logger, ILogger<LogHelper> loghelperLogger)
        {
            this.websiteUserRepo = websiteUserRepo;
            this.logHelper = new LogHelper(logItemRepo, loghelperLogger);
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
            try
            {
                var logItem = new LogItem()
                {
                    Ip = this.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(),
                    Endpoint = "POST /user",
                    Action = $"WebsiteUser with ID: {user.Id}, Name: {user.UserName}",
                    DateTime = DateTime.UtcNow,
                    SubjectUser = "1"
                };

                logHelper.AddUserLog(logItem);

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
            try
            {
                var logItem = new LogItem()
                {
                    Ip = this.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(),
                    Endpoint = $"POST /user/{id}",
                    Action = $"WebsiteUser with ID: {user.Id}, Name: {user.UserName}",
                    DateTime = DateTime.UtcNow,
                    SubjectUser = "1"
                };

                logHelper.AddUserLog(logItem);

                websiteUserRepo.Update(user, id);
                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e);
            }
        }

        [HttpGet("{streamerId}/followers/{followerId}/exists")]
        public IActionResult CheckFollowerExists(int streamerId, int followerId)
        {
            bool followerExists = websiteUserRepo.FollowerExists(streamerId, followerId);
            return Ok(followerExists);
        }
    }
}
