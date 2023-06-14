using TheCircleBackend.Domain.Interfaces;
using TheCircleBackend.Domain.Models;

namespace TheCircleBackend.Domain.DTO
{
    public class UserContentDTO : IContent
    {
        public List<WebsiteUser> OriginalData { get; set; }
        public string ServerPublicKey { get; set; }
    }
}
