using TheCircleBackend.Domain.Models;

namespace TheCircleBackend.DomainServices.IRepo
{
    public interface ILogItemRepo
    {
        public void Add(LogItem logItem);
    }
}
