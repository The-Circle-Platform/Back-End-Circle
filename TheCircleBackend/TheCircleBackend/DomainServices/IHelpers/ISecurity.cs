using System.Security.Cryptography;

namespace TheCircleBackend.DomainServices.IHelpers
{
    public interface ISecurity
    {
        bool HoldsIntegrity(object inputData, byte[] signature, string publicKey);
        (string privKey, string pubKey) GenerateKeys();
        byte[] EncryptData(object inputData, string privateKey);
        byte[] CreateSignature(byte[] inputData, RSAParameters privateKey);
        (RSAParameters privKey, RSAParameters pubKey) GetKeys(int userId);
    }
}
