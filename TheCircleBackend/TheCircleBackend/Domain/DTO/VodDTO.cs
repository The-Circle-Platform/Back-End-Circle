using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TheCircleBackend.Domain.Models;

namespace TheCircleBackend.Domain.DTO
{
    public class VodDTO
    {
        public VodHelper OriginalData { get; set; }
        public string Signature { get; set; }
    }
}
