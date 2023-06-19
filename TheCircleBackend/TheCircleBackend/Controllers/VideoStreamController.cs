using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TheCircleBackend.Domain.DTO;
using TheCircleBackend.Domain.DTO.EncryptedPayload;
using TheCircleBackend.DomainServices.IHelpers;
using TheCircleBackend.DomainServices.IRepo;

namespace TheCircleBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoStreamController : ControllerBase
    {
        private readonly ISecurityService securityService;
        private readonly IVidStreamRepo VidStreamRepo;

        public VideoStreamController(ISecurityService securityService, IVidStreamRepo vidStreamRepo)
        {
            this.securityService = securityService;
            VidStreamRepo = vidStreamRepo;
        }

        

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            //Get videostream
            Domain.Models.Stream? VideoStream = VidStreamRepo.GetById(id);

            var VidStreamDTO = new VideoStreamDTO()
            {
                id = 0,
                endStream = null,
                startStream = new DateTime(),
                transparantUserId = VideoStream.User.Id,
                title = VideoStream.Title,
                transparantUserName = VideoStream.User.UserName
            };

            //Server keypair
            var ServerKeys = securityService.GetServerKeys();

            var Signature = securityService.SignData(VidStreamDTO, ServerKeys.privKey);

            var streamInfoPackage = new VidStreamDTO()
            {
                OriginalData = VidStreamDTO,
                Signature = Signature,
                RandomId = Guid.NewGuid().ToString()
            };
            return Ok(streamInfoPackage);
        }
    }
}
