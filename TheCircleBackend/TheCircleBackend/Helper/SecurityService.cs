using System.Security.Cryptography;
using TheCircleBackend.DBInfra.Repo;
using TheCircleBackend.DomainServices.IHelpers;
using TheCircleBackend.DomainServices.IRepo;

namespace TheCircleBackend.Helper
{
    public class SecurityService : ISecurityService
    {
        private readonly ISecurityHelper securityHelper;
        private readonly IKeyRepo keyRepo;

        public SecurityService(ISecurityHelper securityHelper, IKeyRepo keyRepo)
        {
            this.securityHelper = securityHelper;
            this.keyRepo = keyRepo;
        }

        //Checks if data is tampered.
        public bool HoldsIntegrity(object inputData, byte[] signature, string InputPublicKey)
        {
            try
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
            catch
            {
                return false;
            }
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
        
        public (string privKey, string pubKey) GetKeys(int userId)
        {
            //Get userinfo
            try
            {
                var KeyPair = keyRepo.GetKeys(userId);
                if(KeyPair == null)
                {
                    throw new InvalidOperationException("Keys not found");
                }
                else
                {
                    return KeyPair.Value;
                }
            }
            catch(InvalidOperationException e)
            {
                throw e;
            }
        }

        public byte[] ConvertItemIntoBytes(object item, string key)
        {
            return securityHelper.ConvertItem(item);
        }
    }
}
