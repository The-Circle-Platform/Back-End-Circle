using TheCircleBackend.Domain.Interfaces;

namespace TheCircleBackend.DomainServices.IHelpers
{
    public interface ISecurity
    {

        byte[] SignData(int userId, byte[] input);
        Stream SignStream(int userId, Stream inStream);

        bool VerifySignature(int UserId, Stream inStream);
        bool VerifyBytes(int UserId, byte[] inBytes);

        (string privateKey, string publicKey) GenerateKeys(string signature);

    }
}
