using Microsoft.AspNetCore.Mvc;
using TheCircleBackend.Domain.Models;

namespace TheCircleBackend.DomainServices.IHelpers
{
    public interface ILogHelper
    {
        IActionResult AddUserLog(LogItem logItem);
        IEnumerable<LogItem> GetAllLogItems();
        LogItem GetLogItemById(int id);

    }
}
