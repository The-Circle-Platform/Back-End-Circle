namespace TheCircleBackend.DomainServices.IRepo
{
    public interface IKeyRepo
    {

        string? GetKeys(int UserId);
        bool StoreKeys(int UserId, string pubKey);
    }
}
