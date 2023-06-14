using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TheCircleBackend.Domain.DTO;
using TheCircleBackend.Domain.Models;
using TheCircleBackend.DomainServices.IRepo;
using TheCircleBackend.Helper;

namespace TheCircleBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoggingController : ControllerBase
    {

        private readonly ILogItemRepo logItemRepo;
        private readonly LogHelper logHelper;

        public LoggingController(ILogItemRepo logItemRepo, ILogger<LoggingController> logger)
        {
            this.logItemRepo = logItemRepo;
            this.logHelper = logHelper;
            this.logHelper = new LogHelper(logItemRepo, logger, "LoggingController");
        }

        [HttpPost]
        public IActionResult AddLog(LogDTO dto)
        {
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
                return Ok("Log added");

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500, "Unable to add log");
            }

        }

        [HttpGet]
        public IActionResult GetAllLogItems()
        {
            return Ok(this.logItemRepo.GetAllLogItems());
        }

        [HttpGet ("{id}")]
        public IActionResult GetLogItemById(int id)
        {
            var result = this.logItemRepo.GetLogItemById(id);
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
