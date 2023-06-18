using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TheCircleBackend.Domain.DTO;
using TheCircleBackend.Domain.Models;
using TheCircleBackend.DomainServices.IHelpers;
using TheCircleBackend.Helper;

namespace TheCircleBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoggingController : ControllerBase
    {
        private readonly ILogger<LogHelper> _logger;
        private readonly ILogHelper _logHelper;

        public LoggingController(ILogHelper logHelper, ILogger<LogHelper> logger)
        {
            this._logHelper = logHelper;
            this._logger = logger;
        }

        [HttpPost]
        public IActionResult AddLog(LogDTO dto)
        {
            try
            {
                var log = new LogItem()
                {
                    Action = dto.Action,
                    DateTime = DateTime.Now,
                    Endpoint = "POST /logging",
                    SubjectUser = this.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value,
                    Ip = this.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(),
                };

                this._logHelper.AddUserLog(log);
                return Ok("Log added");

            }
            catch (Exception e)
            {
                return StatusCode(500, "Unable to add log");
            }

        }

        [HttpGet]
        public IActionResult GetAllLogItems()
        {
            return Ok(this._logHelper.GetAllLogItems());
        }

        [HttpGet ("{id}")]
        public IActionResult GetLogItemById(int id)
        {
            var result = this._logHelper.GetLogItemById(id);
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
