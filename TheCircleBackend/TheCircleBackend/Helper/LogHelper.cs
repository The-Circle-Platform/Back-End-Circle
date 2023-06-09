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

        public LogHelper(ILogItemRepo logItemRepo, ILogger logger, string location)
        {
            this.logItemRepo = logItemRepo;
            this.logger = logger;
            this.location = location;
        }


        public void UserLog(string ip, string msg)
        {
            SetLogItem(ip, msg);
            
            this.logItemRepo.Add(logItem);
            //this.logger.LogTrace("DateTime: {date} | IP: {ip} | User: {user} | At: {location} | Action: {msg}", DateTime.Now, ip, 1, location, msg);
        }

        private void SetLogItem(string ip, string msg)
        {
            logItem = new LogItem();
            logItem.DateTime = DateTime.Now;
            logItem.Ip = ip;
            logItem.Location = location;
            logItem.Action = msg;
        }
    }
}
