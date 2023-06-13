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
        private readonly IWebsiteUserRepo websiteUserRepo;
        private readonly ILogItemRepo logItemRepo;
        private readonly LogHelper logHelper;

        public streamController(IWebsiteUserRepo websiteUserRepo, ILogItemRepo logItemRepo, ILogger<streamController> logger)
        {
            this.websiteUserRepo = websiteUserRepo;
            this.logItemRepo = logItemRepo;
            this.logHelper = new LogHelper(logItemRepo, logger, "streamController");

        }

    }
}
