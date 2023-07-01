using System.Security.Cryptography;
using System.Text.Json;
using System.Text;
using System.Xml.Serialization;
using Org.BouncyCastle.Utilities.Encoders;
using TheCircleBackend.DomainServices.IHelpers;
using Org.BouncyCastle.Crypto;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(input).ToLower();
            Console.WriteLine(jsonString);
;           return Encoding.UTF8.GetBytes(jsonString);
        }

        public byte[] DeserialiseKey(string key)
        {
            try
            {
                return Convert.FromBase64String(key);
            }
            catch
            {
                throw new Exception("De-serialization has failed");
            }
            
        }

        public (byte[] privateKey, byte[] publicKey) GenerateKeyPairs()
        {
            var rsaService = new RSACryptoServiceProvider();

            return (rsaService.ExportPkcs8PrivateKey(), rsaService.ExportSubjectPublicKeyInfo());
        }

        // Generates key pair.
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
            var privateKey = "MIICdwIBADANBgkqhkiG9w0BAQEFAASCAmEwggJdAgEAAoGBAMJefUpb8rGnf7WiUtC4funvWKjgLQIoXrUbJItDb2Q5Dq4NAaYMtih/2iq4eABCn9keb+NWe2F2ZcbyI7iLwj+UiLVpCffgj3CfoeYExE1RqLn1S5CXI9kMJNaTsQXPSA/BnjppX+5Z0zAI+TZef44B6NwRIsE/dYb0dtMejT+VAgMBAAECgYAJ+JLw15qxpmgUx0j8UBqioZaowydL7wo8vDG5uzHhsFOidiRZgllt5nEos+HkEYblunv+65bUvyAlfpJ6iyDhzxgs9fSapdkhiz057BVkmwOqzIDDefHjpqh00k+sEZWeZKq0flXG12yF8LI4c1qXnjTnTUCVzIJXhe4kPqufGQJBAMkIY+8QG4EO4RsvOkdS4Bmz+GSZr7n9FLKsWEQU958v99aGC4T8OLaMFpztRrDwj7tZcvEWl7qVHbI5aTrjnbcCQQD3g6oZdQSLEvU4F4NIiUijTMtMgImzKujbhLdchETqrG0G4UUzGl5Itp/NMhLjscsykgl5mlI/4N2We2Hoi80TAkAEP5olDjkWlCLruSbJJRY5VNVWAu10x8VtNTk0TyEgixn4vaJ2sAHe0b0UmesZiCvxcKV+NNUGC2qyPoZbyT2nAkEA4VhTPngmWcQ51Aa8NQcgReS91vnT5HZ1qJ5tHmMiJ5IydSgVi5A/NO5oETa8secGPBVvYPIaXiQJOl885a6aVwJBALIPMLuUps82cKIanFRh0OC8vIZwtFgv8PUpCAYDG1LKo6RDaSoRL7qtighAagxp+pYcU0rQAyuwHf9mHGiyESM=";
            var publicKey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDCXn1KW/Kxp3+1olLQuH7p71io4C0CKF61GySLQ29kOQ6uDQGmDLYof9oquHgAQp/ZHm/jVnthdmXG8iO4i8I/lIi1aQn34I9wn6HmBMRNUai59UuQlyPZDCTWk7EFz0gPwZ46aV/uWdMwCPk2Xn+OAejcESLBP3WG9HbTHo0/lQIDAQAB";
            return (privateKey, publicKey);
        }

        public async Task<string> DecryptMessage(string message)
        {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                Console.WriteLine(message);
                rsa.ImportPkcs8PrivateKey(DeserialiseKey(GetServerKeys().privKey), out _);
                var test = rsa.Decrypt(Convert.FromBase64String(message), false);
                Console.WriteLine(Encoding.UTF8.GetString(test));
                return Encoding.UTF8.GetString(test);

        }
    }
}
