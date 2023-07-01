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
        private readonly IWebsiteUserRepo websiteUserRepo;

        public VideoStreamController(ISecurityService securityService, IVidStreamRepo vidStreamRepo, IWebsiteUserRepo websiteUserRepo)
        {
            this.securityService = securityService;
            VidStreamRepo = vidStreamRepo;
            this.websiteUserRepo = websiteUserRepo;
        }

        [HttpGet("{hostUserName}/2")]
        public IActionResult Get(string hostUserName)
        {
            var user = websiteUserRepo.GetByUserName(hostUserName);

            if(user == null)
            {
                return NotFound();
            }

            //Get videostream
            Domain.Models.Stream? VideoStream = VidStreamRepo.GetCurrentStream(user.Id);

            //Server keypair
            var ServerKeys = securityService.GetServerKeys();

            if(VideoStream == null)
            {
                var sign = securityService.SignData("No running stream found", ServerKeys.privKey);
                var DTO = new
                {
                    Signature = sign,
                    OriginalData = "No running stream found"
                };
                return BadRequest(DTO);
            }

            var VidStreamDTO = new VideoStreamDTO()
            {
                id = VideoStream.Id,
                endStream = null,
                startStream = new DateTime(),
                transparantUserId = user.Id,
                title = VideoStream.Title,
                transparantUserName = user.UserName
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

        [HttpGet("{hostId}")]
        public IActionResult Get(int hostId)
        {
            //Get videostream
            Domain.Models.Stream? VideoStream = VidStreamRepo.GetCurrentStream(hostId);

            Console.WriteLine($"Latest stream of hostId {hostId} with streamId {VideoStream.Id}");
            //Server keypair
            var ServerKeys = securityService.GetServerKeys();

            if (VideoStream == null)
            {
                var sign = securityService.SignData("No running stream found", ServerKeys.privKey);
                var DTO = new
                {
                    Signature = sign,
                    OriginalData = "No running stream found"
                };
                return BadRequest(DTO);
            }

            var TransparantUser = websiteUserRepo.GetById(hostId);

            var VidStreamDTO = new VideoStreamDTO()
            {
                id = VideoStream.Id,
                endStream = null,
                startStream = new DateTime(),
                transparantUserId = hostId,
                title = VideoStream.Title,
                transparantUserName = TransparantUser.UserName
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

        [HttpPost]
        public  IActionResult Post(VidStreamDTO videoStreamDTO)
        {
            //Get userId keys
            var KeyPair = securityService.GetKeys(videoStreamDTO.SenderUserId);
            //check integrity
            var isValid = securityService.HoldsIntegrity(videoStreamDTO.OriginalData, videoStreamDTO.Signature, KeyPair.pubKey);
            //server keys
            var ServerKeys = securityService.GetServerKeys();
            if(isValid)
            {
                VidStreamRepo.StartStream(videoStreamDTO.OriginalData.transparantUserId, videoStreamDTO.OriginalData.title);

                websiteUserRepo.SetUserOnline(videoStreamDTO.OriginalData.transparantUserId);

                var latestStream = VidStreamRepo.GetCurrentStream(videoStreamDTO.OriginalData.transparantUserId);

                Console.WriteLine($"Latest stream created is {latestStream.Id}");
                //Succes response
                var succes = new
                {
                    streamId = latestStream.Id,
                    //Username moet nog toegevoegd worden.
                    userName = latestStream.User.UserName
                };
                var signatureSucces = securityService.SignData(succes, ServerKeys.privKey);
                var succesDTO = new
                {
                    Signature = signatureSucces,
                    OriginalData = succes,
                };
                return Ok(succesDTO);
            }


            //Fail response
            var fail = new
            {
                Message = "Data integrity is not there"
            };

            var signatureOut = securityService.SignData(fail, ServerKeys.privKey);

            var failDTO = new
            {
                Signature = signatureOut,
                OriginalData = fail
            };
            return BadRequest(failDTO);
        }

        [HttpPut("{hostId}/StopStream")]
        public IActionResult Put(int hostId)
        {
            //Server keys
            var ServerKeys = securityService.GetServerKeys();

            // Get recent stream
            var LastStream = VidStreamRepo.GetCurrentStream(hostId);

           //Stream is stopped
            VidStreamRepo.StopStream(hostId, LastStream.Id);

            //Succes response
            var succes = new
            {
                Message = "Data added successfully"
            };
            var signatureSucces = securityService.SignData(succes, ServerKeys.privKey);
            var succesDTO = new
            {
                Signature = signatureSucces,
                OriginalData = succes,
            };
            return Ok(succesDTO);
        }

        [HttpGet("{hostUserName}/StopStream2")]
        public IActionResult Put(string hostUserName)
        {
            //Server keys
            var ServerKeys = securityService.GetServerKeys();

            // Get recent stream
            var user = websiteUserRepo.GetByUserName(hostUserName);

            if(user == null)
            {
                return NotFound();
            }

            var hostId = user.Id; 

            var LastStream = VidStreamRepo.GetCurrentStream(hostId);

            //Stream is stopped
            VidStreamRepo.StopStream(hostId, LastStream.Id);

            //Success response
            var success = new
            {
                Message = "Data added successfully"
            };
            var signatureSucces = securityService.SignData(success, ServerKeys.privKey);
            var succesDTO = new
            {
                Signature = signatureSucces,
                OriginalData = success,
            };
            return Ok(succesDTO);
        }

        [HttpPost("ValidateStream")]
        public IActionResult PostStream(Test inputDTO)
        {
            // Checks timespan in order to prevent replay attacks.
            if((DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - 600000) > inputDTO.OriginalData.TimeStamp)
            {
                return BadRequest("Timeout");
            }
            // Get user by username
            var User = websiteUserRepo.GetByUserName(inputDTO.OriginalData.UserName);
            var serverKeys = securityService.GetServerKeys();


            //If user does not exist return false
            if (User == null)
            {
                // false response
                var sign = securityService.SignData(false, serverKeys.privKey);
                // -  Geef false terug.
                var FalseContent = new NodeStreamOutput
                {
                    //Dummy data
                    Signature = sign,
                    message = "Not found",
                    OriginalData = false
                };
                return BadRequest(FalseContent);
            }

            // Verify digital signature
            var UserKeys = securityService.GetKeys(User.Id);
            bool ValidSignature = securityService.HoldsIntegrity(inputDTO.OriginalData, Convert.FromBase64String(inputDTO.signature), UserKeys.pubKey);

            // signature is valid.
            if (ValidSignature)
            {
                // create stream
                VidStreamRepo.StartStream(User.Id, "Stream of " + inputDTO.OriginalData.UserName);

                //Sets user online
                websiteUserRepo.SetUserOnline(User.Id);

                // - Makes signature with private key of server
                var sign = securityService.SignData(true, serverKeys.privKey);
                // - return true to user
                var GoodContent = new NodeStreamOutput
                {
                    //Dummy data
                    Signature = sign,
                    message = "Access granted",
                    OriginalData = true
                };

                return Ok(GoodContent);
            }
            else
            {
                // Wrong
                
                // - Makes signature
                var sign = securityService.SignData(false, serverKeys.privKey);
                // -  return false
                var FalseContent = new NodeStreamOutput
                {
                    //Dummy data
                    Signature = sign,
                    message = "Access denied",
                    OriginalData = false
                };
                return BadRequest(FalseContent);
            }
        }

    }
}
