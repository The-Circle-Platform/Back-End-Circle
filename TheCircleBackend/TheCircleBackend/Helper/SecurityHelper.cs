using System.Security.Cryptography;
using System.Text.Json;
using System.Text;
using System.Xml.Serialization;
using TheCircleBackend.DomainServices.IHelpers;

namespace TheCircleBackend.Helper
{
    public class SecurityHelper : ISecurityHelper
    {

        public object ConvertFromByteArray(byte[] byteArray, Type targetType)
        {
            string jsonString = Encoding.UTF8.GetString(byteArray);
            return JsonSerializer.Deserialize(jsonString, targetType);

        }

        public byte[] ConvertItem(object input)
        {
            string jsonString = JsonSerializer.Serialize(input);
            return Encoding.UTF8.GetBytes(jsonString);
        }

        public RSAParameters DeserialiseKey(string key)
        {
            try
            {
                using (StringReader reader = new StringReader(key))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(RSAParameters));
                    return (RSAParameters)serializer.Deserialize(reader);
                }
            }
            catch
            {
                throw new Exception("Deserialisatie is misgegaan.");
            }
            
        }

        public (RSAParameters privateKey, RSAParameters publicKey) GenerateKeyPairs()
        {
            var rsaService = new RSACryptoServiceProvider();

            return (rsaService.ExportParameters(true), rsaService.ExportParameters(false));
        }

        // Generates keypair.
        public (string privateKeyString, string publicKeyString) GetKeyString()
        {
            // Generates key pair in RSAParameter form.
            var GeneratedKeys = GenerateKeyPairs();

            // Returns keys.
            return GetKeyString(GeneratedKeys.privateKey, GeneratedKeys.publicKey);
        }

        
        public byte[]? SignData(byte[] DataToSign, RSAParameters Key)
        {
            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

                rsa.ImportParameters(Key);

                // Internally creates a hash of the original message and creates signature based on it, encrypted with private key.
                return rsa.SignData(DataToSign, SHA256.Create());
            }
            catch
            {
                return null;
            }
        }

        public bool VerifySignedData(byte[] DataToVerify, RSAParameters Key, byte[] SignedData)
        {
            try
            {
                // Create a new instance of RSACryptoServiceProvider using the
                // key from RSAParameters.
                RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider();

                RSAalg.ImportParameters(Key);

                // Verify the data using the signature.  Pass a new instance of SHA256
                // to specify the hashing algorithm.
                return RSAalg.VerifyData(DataToVerify, SHA256.Create(), SignedData);
            }
            catch
            {
                return false;
            }
        }

        // OVerloaded-method: Keys are already in RSAParameter format.
        public (string privateKeyString, string publicKeyString) GetKeyString(RSAParameters ExternalPrivateKey, RSAParameters ExternalPublicKey)
        {
            var sw = new StringWriter();
            var sw2 = new StringWriter();
            var xs = new XmlSerializer(typeof(RSAParameters));
            var xs2 = new XmlSerializer(typeof(RSAParameters));
            //Serializing keys
            //Private key string
            xs.Serialize(sw, ExternalPrivateKey);
            //Public key string
            xs2.Serialize(sw2, ExternalPublicKey);

            string privateKey = sw.ToString();
            string publicKey = sw2.ToString();

            return (privateKey, publicKey);
        }

        /*
        public byte[]? SignHash(byte[] encrypted, RSAParameters Key)
        {
            RSACryptoServiceProvider rsaCSP = new RSACryptoServiceProvider();
            SHA1Managed hash = new SHA1Managed();
            byte[] hashedData;

            //Encrypts with private key
            rsaCSP.ImportParameters(Key);

            hashedData = hash.ComputeHash(encrypted);
            return rsaCSP.SignHash(hashedData, CryptoConfig.MapNameToOID("SHA1"));
        }

        public bool VerifyHash(RSAParameters rsaParams, byte[] signedData, byte[] signature)
        {
            RSACryptoServiceProvider rsaCSP = new RSACryptoServiceProvider();
            SHA1Managed hash = new SHA1Managed();
            byte[] hashedData;

            rsaCSP.ImportParameters(rsaParams);
            bool dataOK = rsaCSP.VerifyData(signedData, CryptoConfig.MapNameToOID("SHA1"), signature);
            hashedData = hash.ComputeHash(signedData);
            return rsaCSP.VerifyHash(hashedData, CryptoConfig.MapNameToOID("SHA1"), signature);
        }
        */
    }
}
