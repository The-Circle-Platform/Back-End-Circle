using Microsoft.AspNetCore.Mvc;
using TheCircleBackend.Domain.Models;

namespace TheCircleBackend.DomainServices.IRepo
{
    public interface ILogItemRepo
    {
        public void Add(LogItem logItem);

        public IEnumerable<LogItem> GetAllLogItems();

        public LogItem GetLogItemById(int id);

    }
}
