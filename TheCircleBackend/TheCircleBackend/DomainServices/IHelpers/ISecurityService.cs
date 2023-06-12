namespace TheCircleBackend.DomainServices.IHelpers
{
    public interface ISecurityService
    {
        object ConvertBackToObject(byte[] formerObject);
        byte[] ConvertToBytes(object input);

        (string privateKey, string publicKey) GenerateKeys(string userName);
        byte[] GenerateHash(string input);
        string HashBytesToString(byte[] data);
        byte[] GenerateHash(byte[] input);

        byte[] SignData(int userId, byte[] input);
        Stream SignStream(int userId, Stream inStream);
        bool VerifyStream(int UserId, Stream inStream);
        bool VerifyBytes(int UserId, byte[] inBytes);
        

    }
}
