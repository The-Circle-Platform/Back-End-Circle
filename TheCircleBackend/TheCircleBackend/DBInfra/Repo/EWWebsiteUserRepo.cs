using TheCircleBackend.Domain.Models;
using TheCircleBackend.DomainServices.IRepo;

namespace TheCircleBackend.DBInfra.Repo
{
    public class EWWebsiteUserRepo : IWebsiteUserRepo
    {

        private readonly DomainContext context;
        private readonly IWebsiteUserRepo _websiteUserRepo;

        public EWWebsiteUserRepo(IWebsiteUserRepo _websiteUserRepo, DomainContext context)
        {
            this._websiteUserRepo = _websiteUserRepo;
            this.context = context;
        }
        public void Add(WebsiteUser user)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<WebsiteUser> GetAllWebsiteUsers()
        {
            return context.WebsiteUser;
        }

        public WebsiteUser GetById(int id)
        {
            return context.WebsiteUser.Where(u => u.Id == id).FirstOrDefault();
        }

        public void Update(WebsiteUser user)
        {
            throw new NotImplementedException();
        }
    }
}
