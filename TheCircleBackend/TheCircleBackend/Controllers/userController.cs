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
            this.logHelper = new LogHelper(logItemRepo, logger, "UserController");

        }

        [HttpGet]
        public IActionResult Get([FromBody] UserIncomingDTO userIncomingDTO)
        {
            // Access the Request object
            var UserKeys = securityService.GetKeys(userIncomingDTO.SenderUserId);

            // Checks on integrity
            bool HoldsIntegrity = securityService.HoldsIntegrity(userIncomingDTO.OriginalUserRequest, userIncomingDTO.Signature, UserKeys.pubKey);

            if (!HoldsIntegrity)
            {
                //Sends 400 code back to user.
                return BadRequest();
            }

            // Get all website users.
            var users = websiteUserRepo.GetAllWebsiteUsers().ToList();

            //Create signature
            var Signature = securityService.SignData(users, UserKeys.privKey);
            
            // Packs in dto to client.
            var DTO = new UserContentDTO()
            {
                OriginalList = users,
                PublicKey = UserKeys.pubKey,
                Signature = Signature
            };

            return Ok(DTO);
        }

        [HttpGet("{id}")]
        public IActionResult get([FromBody] UserIncomingDTO userIncomingDTO, int id)
        {
            // Access the Request object
            var UserKeys = securityService.GetKeys(userIncomingDTO.SenderUserId);

            // Checks on integrity
            // Note: This userIncomingDTO contains the userId it wants to request and senderId.
            // If a user searches on Id 6 for example, the DTO contains RequestId = 6. 
            // Those two variabels will be stored in a object and signed by RSA.
            bool HoldsIntegrity = securityService.HoldsIntegrity(userIncomingDTO.OriginalUserRequest, userIncomingDTO.Signature, UserKeys.pubKey);

            if (!HoldsIntegrity)
            {
                //Sends 400 code back to user.
                return BadRequest();
            }

            Console.WriteLine(id);
            var user = websiteUserRepo.GetById(id);
            if (user == null)
            {
                return NotFound();
            }
            //Stores 
            //Create signature
            var Signature = securityService.SignData(user, UserKeys.privKey);

            var DTO = new UserContentDTO()
            {
                OriginalData = user,
                PublicKey = UserKeys.pubKey,
                Signature = Signature
            };

            return Ok(DTO);

        }
    }
}
