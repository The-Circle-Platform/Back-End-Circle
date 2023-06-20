using Org.BouncyCastle.Crypto.Digests;
using System.Reflection.Metadata;

namespace TheCircleBackend.Domain.Models
{
    public class Pfpcs
    {
        public int Id { get; set; }
        public string ImageName { get; set; }
        public string Base64Image { get; set; }
    }
}