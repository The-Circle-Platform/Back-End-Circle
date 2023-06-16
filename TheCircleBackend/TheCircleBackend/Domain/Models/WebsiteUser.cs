using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TheCircleBackend.Domain.Models
{
    public class WebsiteUser
    {
        public int Id { get; set; }
        [Required]
        public string UserName { get; set; }
        public bool IsOnline { get; set; } = false;
        public string? ImageName { get; set; }
        public string? Base64Image { get; set; }

        [JsonIgnore]
        public List<ChatMessage>? UserChatMessages { get; set; }
        
        [JsonIgnore]
        public List<ChatMessage>? StreamChatMessages { get; set; }

        [JsonIgnore]
        public List<Viewer>? CurrentWatchList { get; set; }
       
    }
}
