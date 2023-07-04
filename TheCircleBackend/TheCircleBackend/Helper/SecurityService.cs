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
                //Convert input data to bytes
                byte[] inputBytes = securityHelper.ConvertItem(inputData);

                // Convert key to RSAParameter
                byte[] publicKey = securityHelper.DeserialiseKey(InputPublicKey);

                //Check data.
                bool isValidData = securityHelper.VerifySignedData(inputBytes, publicKey, signature, false);

                //Checks if data is valid and holds integrity
                return isValidData;
            }
            catch
            {
                return false;
            }
        }

        // Generates key pair.
        public (string privKey, string pubKey) GenerateKeys()
        {
            return securityHelper.GetKeyString();
        }

        //Encrypts data, by signing the data with a pri
        public byte[]? SignData(object inputData, string privateKey)
        {

            //   Converts input to a byte array.
            byte[] GeneratedData = securityHelper.ConvertItem(inputData);

            byte[] GeneratedPrivateKey = securityHelper.DeserialiseKey(privateKey);

            return securityHelper.SignData(GeneratedData, GeneratedPrivateKey, true);
        }
        
        public (string privKey, string pubKey) GetKeys(int userId)
        {
            //Get user info
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

        public bool StoreKeys(int UserId, string privKey, string pubKey)
        {
            try
            {
                keyRepo.StoreKeys(UserId, privKey, pubKey);
                return true;
            } catch{
                return false;
            }
        }

        public (string privKey, string pubKey) GetServerKeys()
        {
            return securityHelper.GetServerKeys();
        }

        public string GetVideoServerPublicKey()
        {
            return securityHelper.GetVideoServerPublicKey();
        }

        public async Task<string> DecryptMessage(string message)
        {
            return await this.securityHelper.DecryptMessage(message);
        }
    }
}
