using System.Security.Cryptography;
using TheCircleBackend.DomainServices.IHelpers;

namespace TheCircleBackend.Helper
{
    public class Security : ISecurity
    {
        private readonly ISecurityHelper securityHelper;

        public Security(ISecurityHelper securityHelper)
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
            //Check signature
            bool isValidSignature = this.securityHelper.VerifyHash(publicKey, inputBytes, signature);
            //Check data.
            bool isValidData = this.securityHelper.VerifySignedData(inputBytes, publicKey, signature);
            
            //Checks if data is valid and holds integrity
            return isValidSignature && isValidData;
        }

        // Generates keypair.
        public (string privKey, string pubKey) GenerateKeys()
        {
            var keyPairs = securityHelper.GenerateKeyPairs();

            return securityHelper.GetKeyString(keyPairs.privateKey, keyPairs.publicKey);
        }

        //Encrypts data.
        public byte[]? EncryptData(object inputData, string privateKey)
        {
            byte[] GeneratedData = securityHelper.ConvertItem(inputData);

            RSAParameters GeneratedPrivateKey = securityHelper.DeserialiseKey(privateKey);

            return securityHelper.SignData(GeneratedData, GeneratedPrivateKey);
        }

        //Creates signature.
        public byte[]? CreateSignature(byte[] inputData, RSAParameters privateKey)
        {
            return securityHelper.SignHash(inputData, privateKey);
        }

        public (RSAParameters privKey, RSAParameters pubKey) GetKeys(int userId)
        {
            //Get userinfo

            //Deserialize keystring 
            throw new NotImplementedException();
        }
    }
}
