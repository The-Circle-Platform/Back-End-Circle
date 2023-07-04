using TheCircleBackend.Domain.Models;
using TheCircleBackend.DomainServices.IRepo;

namespace TheCircleBackend.DBInfra.Repo
{
    public class EFKeyRepo : IKeyRepo
    {
        private readonly DomainContext domainContext;
        private readonly ILogger<EFLogItemRepo> logger;

        public EFKeyRepo(DomainContext domainContext, ILogger<EFLogItemRepo> logger) {
            this.domainContext = domainContext;
            this.logger = logger;
        }  
        public string? GetKeys(int UserId)
        {
            try
            {
                var result = domainContext.UserKeys.First(K => K.UserId == UserId);
                return result.PublicKey;
            } catch(Exception ex)
            {
                return null;
            }
        }

        public bool StoreKeys(int UserId, string pubKey)
        {
            try
            {
                domainContext.UserKeys.Add(new KeyStore() {PublicKey = pubKey, UserId = UserId });
                domainContext.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
            
        }
    }
}
