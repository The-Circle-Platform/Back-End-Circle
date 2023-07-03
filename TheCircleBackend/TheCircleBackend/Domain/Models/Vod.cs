using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TheCircleBackend.Domain.Interfaces;

namespace TheCircleBackend.Domain.Models
{
    public class Vod : IDomain
    {
        public int Id { get; set; }
        [Required]
        public string? Title { get; set; }
        [Required]
        public string? ContentType { get; set; }
        [Required]
        public string? Data { get; set; }
    }
}
