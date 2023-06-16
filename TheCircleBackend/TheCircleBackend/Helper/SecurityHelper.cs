using System.Security.Cryptography;
using System.Text.Json;
using System.Text;
using System.Xml.Serialization;
using Org.BouncyCastle.Utilities.Encoders;
using TheCircleBackend.DomainServices.IHelpers;
using Org.BouncyCastle.Crypto;

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

        public byte[] DeserialiseKey(string key)
        {
            try
            {
                return Convert.FromBase64String(key);
            }
            catch
            {
                throw new Exception("Deserialisatie is misgegaan.");
            }
            
        }

        public (byte[] privateKey, byte[] publicKey) GenerateKeyPairs()
        {
            var rsaService = new RSACryptoServiceProvider();

            return (rsaService.ExportPkcs8PrivateKey(), rsaService.ExportSubjectPublicKeyInfo());
        }

        // Generates keypair.
        public (string privateKeyString, string publicKeyString) GetKeyString()
        {
            // Generates key pair in RSAParameter form.
            var GeneratedKeys = GenerateKeyPairs();

            // Returns keys.
            return GetKeyString(GeneratedKeys.privateKey, GeneratedKeys.publicKey);
        }

        
        public byte[]? SignData(byte[] DataToSign, byte[] Key, bool IsPrivate)
        {
            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

                if (IsPrivate)
                {
                    // Set rsa with private key
                    rsa.ImportPkcs8PrivateKey(Key, out _);
                } else
                {
                    // Set rsa with public key
                    rsa.ImportSubjectPublicKeyInfo(Key, out _);
                }
 
                // Internally creates a hash of the original message and creates signature based on it, encrypted with private key.
                return rsa.SignData(DataToSign, SHA256.Create());
            }
            catch
            {
                return null;
            }
        }

        public bool VerifySignedData(byte[] DataToVerify, byte[] Key, byte[] SignedData, bool IsPrivate)
        {
            try
            {
                // Create a new instance of RSACryptoServiceProvider using the
                // key from RSAParameters.
                RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider();

                if (IsPrivate)
                {
                    // Set rsa with private key
                    RSAalg.ImportPkcs8PrivateKey(Key, out _);
                }
                else
                {
                    // Set rsa with public key
                    RSAalg.ImportSubjectPublicKeyInfo(Key, out _);
                }

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
        public (string privateKeyString, string publicKeyString) GetKeyString(byte[] ExternalPrivateKey, byte[] ExternalPublicKey)
        {

            string privateKey = Convert.ToBase64String(ExternalPrivateKey);
            string publicKey = Convert.ToBase64String(ExternalPublicKey);

            return (privateKey, publicKey);
        }

        public (string privKey, string pubKey) GetServerKeys()
        {
            var privateKey = Environment.GetEnvironmentVariable("SERVER_PRIVKEY");
            var publicKey = Environment.GetEnvironmentVariable("SERVER_PUBKEY");
            return (privateKey, publicKey);
        }
    }
}
