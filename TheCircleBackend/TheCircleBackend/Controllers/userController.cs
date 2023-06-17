using Microsoft.AspNetCore.Mvc;
using System.Net;
using TheCircleBackend.Domain.Models;
using TheCircleBackend.DomainServices.IRepo;
using Microsoft.Extensions.Logging;
using TheCircleBackend.Helper;
using System.Security.Claims;
using TheCircleBackend.DomainServices.IHelpers;
using TheCircleBackend.Domain.DTO.EncryptedPayload;

namespace TheCircleBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class userController : ControllerBase
    {

        private readonly IWebsiteUserRepo websiteUserRepo;
        private readonly ILogItemRepo logItemRepo;
        private readonly ISecurityService securityService;
        private readonly LogHelper logHelper;

        public userController(
            IWebsiteUserRepo websiteUserRepo, 
            ILogItemRepo logItemRepo, 
            ILogger<userController> logger,
            ISecurityService securityService)
        {
            this.websiteUserRepo = websiteUserRepo;
            this.logItemRepo = logItemRepo;
            this.securityService = securityService;
            this.logHelper = new LogHelper(logItemRepo, logger);

        }

        [HttpGet]
        public IActionResult Get()
        {
           
            // Get all website users.
            var users = websiteUserRepo.GetAllWebsiteUsers().ToList();

            //Create signature
            var KeyPair = securityService.GetServerKeys();
            var Signature = securityService.SignData(users, KeyPair.privKey);
            
            // Packs in dto to client.
            var DTO = new UserContentDTO()
            {
                OriginalList = users,
                Signature = Signature
            };

            return Ok(DTO);
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
            //Stores 
            //Create signature
            var KeyPair = securityService.GetServerKeys();
            var Signature = securityService.SignData(user, KeyPair.privKey);

            var DTO = new UserContentDTO()
            {
                OriginalData = user,
                Signature = Signature
            };

            return Ok(DTO);

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
