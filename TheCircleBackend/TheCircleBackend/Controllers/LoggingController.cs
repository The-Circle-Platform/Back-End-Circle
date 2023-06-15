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
            this.logHelper = new LogHelper(logItemRepo, logger, "LoggingController");
        }

        [HttpPost]
        public IActionResult AddLog(LoggingPayloadDTO payload)
        {
            //Get keypair
            var KeyPair = securityService.GetKeys(payload.SenderUserId);

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
                    PublicKey = KeyPair.pubKey
                };
                return BadRequest(load);
            }

            var dto = payload.OriginalData;
            var log = new LogItem()
            {
                Action = dto.Action,
                DateTime = DateTime.Now,
                Ip = this.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(),
                Location = dto.Location
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
                    SenderUserId = payload.SenderUserId,
                    PublicKey = KeyPair.pubKey
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
                    SenderUserId = payload.SenderUserId,
                    PublicKey = KeyPair.pubKey
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
            var KeyPair = securityService.GenerateKeys();

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
        public IActionResult GetLogItemById(int id, LoggingIdDTO dTO)
        {
            // Get keypair of user
            var KeyPair = securityService.GetKeys(dTO.SenderUserId);

            // Check integrity
            var isValid =securityService.HoldsIntegrity(id, dTO.Signature, KeyPair.pubKey);

            var result = this.logItemRepo.GetLogItemById(id);
            
            if (result != null && isValid)
            {
                var Sign = securityService.SignData (result, KeyPair.privKey);
                var load = new LoggingOutDTO()
                {
                    OriginalData = result,
                    Signature = Sign,
                    PublicKey = KeyPair.pubKey,
                    SenderUserId = dTO.SenderUserId,
                };
                return Ok(load);
            }
            else
            {
                var GeneralKeyPair = securityService.GenerateKeys();
                var Sign = securityService.SignData("Integriteit is belast", GeneralKeyPair.privKey);
                var load = new LoggingOutTextDTO()
                {
                    OriginalData = "Integriteit is belast",
                    Signature = Sign,
                    PublicKey = GeneralKeyPair.pubKey,
                    SenderUserId = dTO.SenderUserId,
                };
                return NotFound(load);
            }
        }
    }
}
