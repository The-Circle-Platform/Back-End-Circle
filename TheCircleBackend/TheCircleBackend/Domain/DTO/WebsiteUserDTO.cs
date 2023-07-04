using System.ComponentModel.DataAnnotations;

namespace TheCircleBackend.Domain.DTO
{
    public class WebsiteUserDTORequest
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public bool IsOnline { get; set; } = false;
        public int FollowCount { get; set; }
        public int Balance { get; set; }
        public string ImageName { get; set; }   
        public string Base64Image { get; set; }
        public long? TimeStamp { get; set; }


    }

    public class WebsiteUserDTO
    {
        public WebsiteUserDTORequest Request { get; set; }
        public string Signature { get; set; }
    }
}
