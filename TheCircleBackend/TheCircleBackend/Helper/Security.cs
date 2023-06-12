using PgpCore;
using TheCircleBackend.Domain.AuthModels;
using TheCircleBackend.DomainServices.IHelpers;

namespace TheCircleBackend.Helper
{
    public class Security : ISecurity
    {
        //Repo
        public Security()
        {

        }

        (string privateKey, string publicKey) ISecurity.GenerateKeys(string signature)
        {
            using PGP pgp = new PGP();

            using var privKeyStream = new MemoryStream();
            using var pubKeyStream = new MemoryStream();

            pgp.GenerateKey(pubKeyStream, privKeyStream, signature);

            privKeyStream.Seek(0, SeekOrigin.Begin);
            pubKeyStream.Seek(0, SeekOrigin.Begin);

            using var pubKeyReader = new StreamReader(pubKeyStream);
            using var privKeyReader = new StreamReader(privKeyStream);

            string privKey = privKeyReader.ReadToEnd();
            string pubKey = pubKeyReader.ReadToEnd();

            return (privKey, pubKey);
        }

        byte[] ISecurity.SignData(int userId, byte[] input)
        {
            Stream stream = new MemoryStream(input);

            var signedStream = SignStream(userId, stream);

            //SignBytes
            byte[] content = new byte[signedStream.Length];
            long numBytesToRead = content.Length;

            int numBytesRead = 0;

            do
            {
                int n = signedStream.Read(content, 0, (int)numBytesToRead);
                numBytesRead += n;
                numBytesToRead -= n;
            } while (numBytesToRead > 0);

            return content;
        }

        Stream SignStream(int userId, Stream inStream)
        {
            AuthUser authUser = null;

            if(authUser == null)
            {
                throw new Exception("UserId with ${authUser.id} not found");
            }
            var signedStream = new MemoryStream();
            // TODO must do something about public and private keys.
            /*EncryptionKeys encryptionKeys = new EncryptionKeys(authUser.PubKey, authUser.PrivKey, string.Empty);
            var pgp = new PGP(encryptionKeys);
            
            pgp.SignStream(inStream, signedStream);
            signedStream.Seek(0, SeekOrigin.Begin);*/

            return signedStream;
        }

        bool ISecurity.VerifyBytes(int UserId, byte[] inBytes)
        {
            Stream stream = new MemoryStream(inBytes);
            return VerifySignature(UserId, stream);
        }

        public bool VerifySignature(int UserId, Stream inStream)
        {
            //Get user
            AuthUser user = null;

            if(user == null)
            {
                return false;
            }
            //Key must be inserted
            /* EncryptionKeys publicKey = new EncryptionKeys(user.pubKey);
             var pgp2 = new PGP(publicKey);

             return pgp2.VerifyStream(inStream);*/
            return true;
        }

        Stream ISecurity.SignStream(int userId, Stream inStream)
        {
            throw new NotImplementedException();
        }
    }
}
