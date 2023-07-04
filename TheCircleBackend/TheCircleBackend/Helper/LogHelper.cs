using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.Extensions.Logging;
using TheCircleBackend.Domain.Models;
using TheCircleBackend.DomainServices.IRepo;

namespace TheCircleBackend.Helper
{
    public class LogHelper : ControllerBase
    {
        private readonly ILogger logger;
        private readonly string location;
        private LogItem logItem;
        private readonly ILogItemRepo logItemRepo;

        public LogHelper(ILogItemRepo logItemRepo, ILogger logger)
        {
            this.logItemRepo = logItemRepo;
            this.logger = logger;
        }


        public IActionResult AddUserLog(string ip, string endpoint, string subjectUser, string msg)
        {
            SetLogItem(ip, endpoint, subjectUser, msg);
            try
            {
                this.logItemRepo.Add(logItem);
                return Ok("Log added");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e);
            }

        }

        private void SetLogItem(string ip, string endpoint, string subjectUser, string msg)
        {
            logItem = new LogItem();
            logItem.DateTime = DateTime.Now;
            logItem.Ip = ip;
            logItem.Endpoint = endpoint;
            logItem.SubjectUser = subjectUser;
            logItem.Action = msg;
        }
    }
}
