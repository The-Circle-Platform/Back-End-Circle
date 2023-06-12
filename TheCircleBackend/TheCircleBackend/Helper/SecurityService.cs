using System.Text;
using System.Text.Json;
using TheCircleBackend.Domain.Models;
using TheCircleBackend.DomainServices.IHelpers;

namespace TheCircleBackend.Helper
{
    public class SecurityService : ISecurityService
    {
        private readonly ISecurity security;

        public SecurityService(ISecurity security)
        {
            this.security = security;
        }

        byte[] ISecurityService.GenerateHash(string input)
        {
            throw new NotImplementedException();
        }

        byte[] ISecurityService.GenerateHash(byte[] input)
        {
            throw new NotImplementedException();
        }

        string ISecurityService.HashBytesToString(byte[] data)
        {
            throw new NotImplementedException();
        }

        public System.IO.Stream SignStream(int userId, System.IO.Stream inStream)
        {
            return this.security.SignStream(userId, inStream);
        }

        public bool VerifyStream(int UserId, System.IO.Stream inStream)
        {
            return this.security.VerifySignature(UserId, inStream);
        }

        object? ISecurityService.ConvertBackToObject(byte[] formerObject)
        {
            string encodedBytes = Encoding.UTF8.GetString(formerObject);

            object output = JsonSerializer.Deserialize<ChatMessage>(encodedBytes);

            return output;
        }

        byte[] ISecurityService.ConvertToBytes(object input)
        {
            string jsonString = JsonSerializer.Serialize(input);

            return Encoding.UTF8.GetBytes(jsonString);
        }

        

        (string privateKey, string publicKey) ISecurityService.GenerateKeys(string userName)
        {
            return this.security.GenerateKeys(userName);
        }

        

        byte[] ISecurityService.SignData(int userId, byte[] input)
        {
            return this.security.SignData(userId, input);
        }


        bool ISecurityService.VerifyBytes(int UserId, byte[] inBytes)
        {
            return this.security.VerifyBytes(UserId, inBytes);
        }

    }
}
