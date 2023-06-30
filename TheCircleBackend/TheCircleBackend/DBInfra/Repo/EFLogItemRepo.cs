using TheCircleBackend.Domain.Models;
using TheCircleBackend.DomainServices.IRepo;

namespace TheCircleBackend.DBInfra.Repo
{
    public class EFLogItemRepo : ILogItemRepo
    {
        private readonly DomainContext context;
        private readonly ILogger<EFLogItemRepo> logger;

        public EFLogItemRepo(DomainContext context, ILogger<EFLogItemRepo> logger)
        {
            this.context = context;
            this.logger = logger;
        }
        public void Add(LogItem logItem)
        {
            context.LogItem.Add(logItem);
            context.SaveChanges();
        }

        public IEnumerable<LogItem> GetAllLogItems()
        {
            return context.LogItem;
        }

        public LogItem GetLogItemById(int id)
        {
            return context.LogItem.Where(l => l.Id == id).FirstOrDefault();
        }
    }
}
