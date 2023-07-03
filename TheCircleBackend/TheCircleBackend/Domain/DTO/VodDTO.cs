using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TheCircleBackend.Domain.DTO
{
    public class VodDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ContentType { get; set; }
        public string Data { get; set; }
    }
}
