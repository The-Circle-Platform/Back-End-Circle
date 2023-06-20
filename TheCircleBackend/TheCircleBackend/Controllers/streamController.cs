using Microsoft.AspNetCore.Mvc;
using System.Net;
using TheCircleBackend.Domain.Models;
using TheCircleBackend.DomainServices.IRepo;
using Microsoft.Extensions.Logging;
using TheCircleBackend.Helper;

namespace TheCircleBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class streamController : ControllerBase
    {
        private readonly IStreamRepository streamRepository;
        private readonly ILogItemRepo logItemRepo;
        private readonly LogHelper logHelper;

        public streamController(IStreamRepository streamRepository, ILogItemRepo logItemRepo, ILogger<streamController> logger)
        {
            this.streamRepository = streamRepository;
            this.logItemRepo = logItemRepo;
            this.logHelper = new LogHelper(logItemRepo, logger, "streamController");

        }

    }
}
