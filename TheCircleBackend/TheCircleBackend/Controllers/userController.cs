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

            var dtoList = new List<WebsiteUserDTO>();

            foreach (var user in users)
            {
                var userDTO = new WebsiteUserDTO()
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    IsOnline = user.IsOnline,
  
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

        [HttpGet("{id}")]
        public IActionResult get(int id)
        {
            Console.WriteLine(id);
            var user = websiteUserRepo.GetById(id);



            if (user == null)
            {
                return NotFound();
            }
            //putting in DTO for signature
            var userDTO = new WebsiteUserDTO()
            {
                Id = user.Id,
                UserName = user.UserName,
                IsOnline = user.IsOnline,
            };
            //Stores 
            //Create signature
            var KeyPair = securityService.GetServerKeys();
            var Signature = securityService.SignData(userDTO, KeyPair.privKey);

            var DTO = new UserContentDTO()
            {
                OriginalData = user,
                Signature = Signature
            };

            return Ok(DTO);

        }
    }
}
