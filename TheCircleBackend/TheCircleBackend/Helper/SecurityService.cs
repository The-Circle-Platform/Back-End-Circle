using System.Security.Cryptography;
using TheCircleBackend.DomainServices.IHelpers;

namespace TheCircleBackend.Helper
{
    public class SecurityService : ISecurityService
    {
        private readonly ISecurityHelper securityHelper;

        public SecurityService(ISecurityHelper securityHelper)
        {
            this.securityHelper = securityHelper;
        }

        //Checks if data is tampered.
        public bool HoldsIntegrity(object inputData, byte[] signature, string InputPublicKey)
        {
            //Convert inputdata to bytes
            byte[] inputBytes = securityHelper.ConvertItem(inputData);
            
            // Convert key to RSAParameter
            RSAParameters publicKey = securityHelper.DeserialiseKey(InputPublicKey);

            //Check data.
            bool isValidData = securityHelper.VerifySignedData(inputBytes, publicKey, signature);
            
            //Checks if data is valid and holds integrity
            return isValidData;
        }

        // Generates keypair.
        public (string privKey, string pubKey) GenerateKeys()
        {
            return securityHelper.GetKeyString();
        }

        //Encrypts data, by signing the data with a pri
        public byte[]? EncryptData(object inputData, string privateKey)
        {

            //   Converts input to a byte array.
            byte[] GeneratedData = securityHelper.ConvertItem(inputData);

            RSAParameters GeneratedPrivateKey = securityHelper.DeserialiseKey(privateKey);

            return securityHelper.SignData(GeneratedData, GeneratedPrivateKey);
        }
        
        public (RSAParameters privKey, RSAParameters pubKey) GetKeys(int userId)
        {
            //Get userinfo

            //Deserialize keystring 
            throw new NotImplementedException();
        }

        public byte[] ConvertItemIntoBytes(object item, string key)
        {
            return securityHelper.ConvertItem(item);
        }

        public (string privKey, string pubKey) GetUserKeys(int id)
        {
            throw new NotImplementedException();
        }
    }
}
