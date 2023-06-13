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
        public (string privKey, string pubKey)? GetKeys(int UserId)
        {
            try
            {
                var result = domainContext.Key.First(K => K.UserId == UserId);
                return (result.PrivateKey, result.PublicKey);
            } catch(Exception ex)
            {
                return null;
            }
        }

        public bool StoreKeys(int UserId, string privKey, string pubKey)
        {
            try
            {
                domainContext.Key.Add(new KeyStore() { PrivateKey = privKey, PublicKey = pubKey, UserId = UserId });
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
