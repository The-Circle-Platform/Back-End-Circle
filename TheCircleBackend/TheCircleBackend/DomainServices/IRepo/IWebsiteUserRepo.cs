using TheCircleBackend.Domain.Models;

namespace TheCircleBackend.DomainServices.IRepo
{
    public interface IWebsiteUserRepo
    {
        public void Add(WebsiteUser user);
        public IEnumerable<WebsiteUser> GetAllWebsiteUsers();
        public WebsiteUser GetById(int id);
        public void Update(WebsiteUser user);
    }
}
