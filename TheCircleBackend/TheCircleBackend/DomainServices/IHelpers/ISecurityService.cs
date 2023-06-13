using System.Security.Cryptography;

namespace TheCircleBackend.DomainServices.IHelpers
{
    public interface ISecurityHelper
    {
        RSAParameters DeserialiseKey(string key);
        (string privateKeyString, string publicKeyString) GetKeyString(RSAParameters ExternalPrivateKey, RSAParameters ExternalPublicKey);
        (RSAParameters privateKey, RSAParameters publicKey) GenerateKeyPairs();
        byte[] ConvertItem(object input);
        object ConvertFromByteArray(byte[] byteArray, Type targetType);

        bool VerifySignedData(byte[] DataToVerify, RSAParameters Key, byte[] SignedData);
        bool VerifyHash(RSAParameters rsaParams, byte[] signedData, byte[] signature);
        byte[]? SignData(byte[] DataToSign, RSAParameters Key);
        byte[]? SignHash(byte[] encrypted, RSAParameters Key);
    }
}
