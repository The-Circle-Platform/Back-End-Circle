using TheCircleBackend.Domain.Models;
using TheCircleBackend.DomainServices.IRepo;

namespace TheCircleBackend.DBInfra.Repo
{
    public class EFWebsiteUserRepo : IWebsiteUserRepo
    {
        private readonly DomainContext context;
        //private readonly IWebsiteUserRepo _websiteUserRepo;

        public EFWebsiteUserRepo(DomainContext context)
        {
            //this._websiteUserRepo = _websiteUserRepo;
            this.context = context;
        }
        public void Add(WebsiteUser user)
        {
            context.WebsiteUser.Add(user);
            context.SaveChanges();
        }

        public IEnumerable<WebsiteUser> GetAllWebsiteUsers()
        {
            return context.WebsiteUser;
        }

        public WebsiteUser GetById(int id)
        {
            return context.WebsiteUser.Where(u => u.Id == id).FirstOrDefault();
        }
        public void Update(WebsiteUser user, int userId)
        {
            var result = context.WebsiteUser.Where(u => u.Id == userId).FirstOrDefault();
            Console.WriteLine(result.UserName);
            if (result != null)
            {
                result.IsOnline = user.IsOnline;
                result.UserName = user.UserName;
                context.SaveChanges();
            }
        }

    }
}
