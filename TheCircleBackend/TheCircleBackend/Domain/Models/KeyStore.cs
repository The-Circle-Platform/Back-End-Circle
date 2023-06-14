using System.ComponentModel.DataAnnotations.Schema;
using TheCircleBackend.Domain.Interfaces;

namespace TheCircleBackend.Domain.Models
{
    public class KeyStore: IDomain
    {
        public int Id { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }

        public int UserId { get; set; }
    }
}
