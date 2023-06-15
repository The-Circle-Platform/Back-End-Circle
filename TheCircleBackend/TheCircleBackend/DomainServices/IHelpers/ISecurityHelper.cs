using System.Security.Cryptography;

namespace TheCircleBackend.DomainServices.IHelpers
{
    public interface ISecurityHelper
    {
        byte[] DeserialiseKey(string key);
        (string privateKeyString, string publicKeyString) GetKeyString(byte[] ExternalPrivateKey, byte[] ExternalPublicKey);
        (string privateKeyString, string publicKeyString) GetKeyString();
        
        byte[] ConvertItem(object input);
        object ConvertFromByteArray(byte[] byteArray, Type targetType);

        bool VerifySignedData(byte[] DataToVerify, byte[] Key, byte[] SignedData, bool IsPrivate);
        byte[]? SignData(byte[] DataToSign, byte[] Key, bool IsPrivate);
    }
}
