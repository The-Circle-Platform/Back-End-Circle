using System.Security.Cryptography;

namespace TheCircleBackend.DomainServices.IHelpers
{
    public interface ISecurityHelper
    {
        RSAParameters DeserialiseKey(string key);
        (string privateKeyString, string publicKeyString) GetKeyString(RSAParameters ExternalPrivateKey, RSAParameters ExternalPublicKey);
        (string privateKeyString, string publicKeyString) GetKeyString();
        
        byte[] ConvertItem(object input);
        object ConvertFromByteArray(byte[] byteArray, Type targetType);

        bool VerifySignedData(byte[] DataToVerify, RSAParameters Key, byte[] SignedData);
        byte[]? SignData(byte[] DataToSign, RSAParameters Key);
        string? EncryptData(object payload, string privateKey);
        byte[]? DecryptData(string payload, string publicKey);
    }
}
