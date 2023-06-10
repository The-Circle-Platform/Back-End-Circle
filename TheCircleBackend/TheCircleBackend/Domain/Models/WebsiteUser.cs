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

        [JsonIgnore]
        public List<ChatMessage>? UserChatMessages { get; set; }

        
    }
}
