using TheCircleBackend.Domain.Models;
using TheCircleBackend.DomainServices.IRepo;

namespace TheCircleBackend.DBInfra.Repo
{
    public class EFVodRepo : IVodRepo
    {
        private readonly DomainContext context;

        public EFVodRepo(DomainContext context, ILogger<EFWebsiteUserRepo> logger)
        {
            this.context = context;
        }

        public Vod? GetVodById(int id)
        {
            return context.Vod.Where(u => u.Id == id).FirstOrDefault();
        }

        public IEnumerable<Vod> GetVods()
        {
            return context.Vod;
        }

        public void AddVod(Vod vod)
        {
            context.Vod.Add(vod);
            context.SaveChanges();
        }

        public bool VideoExists(string fileName)
        {
            return context.Vod.Any(v => v.Title == fileName);
        }
    }
}
