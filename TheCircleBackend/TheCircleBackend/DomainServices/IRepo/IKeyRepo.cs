namespace TheCircleBackend.DomainServices.IRepo
{
    public interface IKeyRepo
    {

        (string privKey, string pubKey)? GetKeys(int UserId);
        bool StoreKeys(int UserId, string privKey, string pubKey);
    }
}
