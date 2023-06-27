using TheCircleBackend.Domain.Models;
using TheCircleBackend.DomainServices.IRepo;

namespace TheCircleBackend.DBInfra.Repo
{
    public class EFWebsiteUserRepo : IWebsiteUserRepo
    {
        private readonly DomainContext context;
        private readonly ILogger<EFWebsiteUserRepo> logger;

        //private readonly IWebsiteUserRepo _websiteUserRepo;

        public EFWebsiteUserRepo(DomainContext context, ILogger<EFWebsiteUserRepo> logger)
        {
            //this._websiteUserRepo = _websiteUserRepo;
            this.context = context;
            this.logger = logger;
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

        public WebsiteUser? GetById(int id)
        {
            this.logger.LogInformation("Get user with id {0}", id);
            return context.WebsiteUser.Where(u => u.Id == id).FirstOrDefault();
        }

        public WebsiteUser? GetByUserName(string UserName)
        {
            return context.WebsiteUser.Where(u => u.UserName == UserName).FirstOrDefault();
        }

        public void SetUserOnline(int userId)
        {
            var user = context.WebsiteUser.First(u => u.Id == userId);

            user.IsOnline = true;

            context.WebsiteUser.Update(user);
            context.SaveChanges();
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
