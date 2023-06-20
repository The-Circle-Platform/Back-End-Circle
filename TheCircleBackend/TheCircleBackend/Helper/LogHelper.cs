using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.Extensions.Logging;
using TheCircleBackend.Domain.Models;
using TheCircleBackend.DomainServices.IRepo;
using TheCircleBackend.DomainServices.IHelpers;
using TheCircleBackend.Domain.DTO;
using TheCircleBackend.DBInfra.Repo;

namespace TheCircleBackend.Helper
{
    public class LogHelper : ControllerBase, ILogHelper
    {
        private readonly ILogger _logger; 
        //private readonly string location;
        private readonly ILogItemRepo _logItemRepo;

        public LogHelper(ILogItemRepo logItemRepo, ILogger<LogHelper> logger)
        {
            this._logItemRepo = logItemRepo;
            this._logger = logger;

        }

        public LogHelper(EFLogItemRepo repo, ILogger<EFLogItemRepo> @object)
        {
        }

        // TODO: Change LogItem to DTO if neccessary
        public IActionResult AddUserLog(LogItem logItem)
        {
            try
            {
                this._logItemRepo.Add(logItem);
                return Ok("Log added");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error adding user log");
                return BadRequest(e);
            }
        }

        public IEnumerable<LogItem> GetAllLogItems()
        {
            return _logItemRepo.GetAllLogItems();
        }

        public LogItem GetLogItemById(int id)
        {
            return _logItemRepo.GetLogItemById(id);
        }
    }
}
