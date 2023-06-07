using System.ComponentModel.DataAnnotations;

namespace TheCircleBackend.Domain.Models
{
    public class WebsiteUser
    {
        public int Id { get; set; }
        [Required]
        public string UserName { get; set; }
        public bool IsOnline { get; set; } = false;
    }
}
