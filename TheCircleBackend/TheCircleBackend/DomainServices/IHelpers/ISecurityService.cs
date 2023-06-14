using System.Security.Cryptography;

namespace TheCircleBackend.DomainServices.IHelpers
{
    public interface ISecurityService
    {
        bool HoldsIntegrity(object inputData, byte[] signature, string publicKey);
        (string privKey, string pubKey) GenerateKeys();
        (string privKey, string pubKey) GetUserKeys(int id);
        byte[] EncryptData(object inputData, string privateKey);
        byte[] ConvertItemIntoBytes(object item, string key);
        // byte[] EncryptHash(byte[] inputData, string privateKey);
        (RSAParameters privKey, RSAParameters pubKey) GetKeys(int userId);
    }
}
