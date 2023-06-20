using System.ComponentModel.DataAnnotations;

namespace TheCircleBackend.Domain.DTO
{
    public class WebsiteUserDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public bool IsOnline { get; set; } = false;
        public int FollowCount { get; set; }
        //public string Email { get; set; }
        public int Balance { get; set; }
        public string ImageName { get; set; }   
        public string Base64Image { get; set; }
    }
}
