using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TheCircleBackend.Domain.DTO;
using TheCircleBackend.Domain.DTO.EncryptedPayload;
using TheCircleBackend.Domain.Models;
using TheCircleBackend.DomainServices.IHelpers;
using TheCircleBackend.DomainServices.IRepo;
using TheCircleBackend.Helper;

namespace TheCircleBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoggingController : ControllerBase
    {

        private readonly ILogItemRepo logItemRepo;
        private readonly ISecurityService securityService;
        private readonly LogHelper logHelper;

        public LoggingController(
            ILogItemRepo logItemRepo, 
            ILogger<LoggingController> logger,
            ISecurityService securityService)
        {
            this.logItemRepo = logItemRepo;
            this.securityService = securityService;
            this.logHelper = new LogHelper(logItemRepo, logger);
        }

        [HttpPost]
        public IActionResult AddLog(LoggingPayloadDTO payload)
        {
            Console.WriteLine("Payload received");
            //Get keypair
            var KeyPair = securityService.GetKeys(payload.SenderUserId);

            Console.WriteLine("Keypair received");
            //Check signature
            var IsValid = securityService.HoldsIntegrity(payload.OriginalData, payload.Signature, KeyPair.pubKey);
            if(!IsValid)
            {
                var sign = securityService.SignData("Integrity is tainted", KeyPair.privKey);

                var load = new LoggingOutTextDTO()
                {
                    OriginalData = "Integrity is tainted",
                    Signature = sign,
                    SenderUserId = payload.SenderUserId,
                };
                return BadRequest(load);
            }

            Console.WriteLine("Keypair received");
            var dto = payload.OriginalData;
            var log = new LogItem()
            {
                Action = dto.Action,
                DateTime = DateTime.Now,
                Ip = this.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(),
                Endpoint = dto.Endpoint,
                SubjectUser = dto.SubjectUser
            };
            try
            {
                this.logItemRepo.Add(log);

                //Create signature
                var sign = securityService.SignData("Log added", KeyPair.privKey);

                var load = new LoggingOutTextDTO()
                {
                    OriginalData = "Log added",
                    Signature = sign,
                    SenderUserId = payload.SenderUserId
                };

                return Ok(load);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                var error = "Integrity is tainted";
                var sign = securityService.SignData(error, KeyPair.privKey);

                var load = new LoggingOutTextDTO()
                {
                    OriginalData = error,
                    Signature = sign,
                    SenderUserId = payload.SenderUserId
                };
                return StatusCode(500, load);
            }

        }

        [HttpGet]
        public IActionResult GetAllLogItems()
        {
            // Gets all logs
            var list = this.logItemRepo.GetAllLogItems();

            // Creates server keys
            var KeyPair = securityService.GetServerKeys();

            //Create signature
            var Sign = securityService.SignData(list, KeyPair.privKey);

            //Create payload to send back
            var load = new
            {
                OriginalData = list,
                PublicKey = KeyPair.pubKey,
                Signature = Sign
            };
            return Ok(load);
        }

        [HttpGet ("{id}")]
        public IActionResult GetLogItemById(int id)
        {

            var result = this.logItemRepo.GetLogItemById(id);
            var ServerKey = securityService.GetServerKeys();

            if (result != null)
            {
                var Sign = securityService.SignData (result, ServerKey.privKey);
                var load = new LoggingOutDTO()
                {
                    OriginalData = result,
                    Signature = Sign,
                };
                return Ok(load);
            }
            else
            {
                var Sign = securityService.SignData("Integriteit is belast", ServerKey.privKey);
                var load = new LoggingOutTextDTO()
                {
                    OriginalData = "Integriteit is belast",
                    Signature = Sign
                };
                return NotFound(load);
            }
        }
    }
}
