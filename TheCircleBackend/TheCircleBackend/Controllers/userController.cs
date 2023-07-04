using Microsoft.AspNetCore.Mvc;
using System.Net;
using TheCircleBackend.Domain.Models;
using TheCircleBackend.DomainServices.IRepo;
using Microsoft.Extensions.Logging;
using TheCircleBackend.Helper;
using System.Security.Claims;
using TheCircleBackend.Domain.DTO;
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
        private readonly IEntityCheckerService entityCheckerService;
        private readonly LogHelper logHelper;

        public userController(
            IWebsiteUserRepo websiteUserRepo, 
            ILogItemRepo logItemRepo, 
            ILogger<userController> logger,
            ISecurityService securityService,
            IEntityCheckerService entityCheckerService)
        {
            this.websiteUserRepo = websiteUserRepo;
            this.logItemRepo = logItemRepo;
            this.securityService = securityService;
            this.entityCheckerService = entityCheckerService;
            this.logHelper = new LogHelper(logItemRepo, logger);

        }

        [HttpGet]
        public IActionResult Get()
        {
           
            // Get all website users.
            var users = websiteUserRepo.GetAllWebsiteUsers().ToList();

            var dtoList = new List<WebsiteUserDTORequest>();

            foreach (var user in users)
            {
                var userDTO = new WebsiteUserDTORequest()
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    IsOnline = user.IsOnline,
                    ImageName = user.ImageName,
                    Base64Image = user.Base64Image,
                    Balance = user.Balance,
  
                };
                dtoList.Add(userDTO);
            }

            //Create signature
            var KeyPair = securityService.GetServerKeys();
            var Signature = securityService.SignData(dtoList, KeyPair.privKey);
            
            // Packs in dto to client.
            var DTO = new 
            {
                OriginalList = dtoList,
                Signature = Signature
            };

            return Ok(DTO);
        }

        [HttpPut("{id}/pfp")]
        public IActionResult PostImage(WebsiteUserDTO websiteUser, int id)
        {
            try
            {
                var key = securityService.GetKeys(websiteUser.Request.Id);
                Console.WriteLine(key);
                var isSameUser = securityService.HoldsIntegrity(websiteUser.Request, Convert.FromBase64String(websiteUser.Signature),
                    key);
                Console.WriteLine(isSameUser);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            

            var user = this.websiteUserRepo.GetById(id);

            var ip = this.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            var endpoint = "POST /user";

            try
            {
                user.Base64Image = websiteUser.Request.Base64Image;
                this.websiteUserRepo.Update(user, id);
                return Ok("user updated");
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            bool isValid = entityCheckerService.UserExists(id);

            if (!isValid)
            {
                return NotFound();
            }

            Console.WriteLine(id);
            var user = websiteUserRepo.GetById(id);



            if (user == null)
            {
                return NotFound();
            }
            //putting in DTO for signature
            var userDTO = new WebsiteUserDTORequest()
            {
                Id = user.Id,
                UserName = user.UserName,
                IsOnline = user.IsOnline,
                Base64Image = user.Base64Image,
                ImageName = user.ImageName,
                Balance = user.Balance,
                TimeStamp = null
            };
            
            //Stores 
            //Create signature
            var KeyPair = securityService.GetServerKeys();
            var Signature = securityService.SignData(userDTO, KeyPair.privKey);

            var DTO = new
            {
                OriginalData = userDTO,
                Signature = Signature
            };

            return Ok(DTO);

        }
        [HttpGet("{id}/pfp")]
        public IActionResult GetPfp(WebsiteUser pfp)
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
