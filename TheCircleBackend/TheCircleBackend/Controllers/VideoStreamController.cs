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

        [HttpGet("{hostId}")]
        public IActionResult Get(int hostId)
        {
            //Get videostream
            Domain.Models.Stream? VideoStream = VidStreamRepo.GetCurrentStream(hostId);
            //Server keypair
            var ServerKeys = securityService.GetServerKeys();

            if(VideoStream == null)
            {
                var sign = securityService.SignData("Geen runnende stream aanwezig", ServerKeys.privKey);
                var DTO = new
                {
                    Signature = sign,
                    OriginalData = "Geen runnende stream aanwezig"
                };
                return BadRequest(DTO);
            }

            var VidStreamDTO = new VideoStreamDTO()
            {
                id = 0,
                endStream = null,
                startStream = new DateTime(),
                transparantUserId = hostId,
                title = VideoStream.Title
            };

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
