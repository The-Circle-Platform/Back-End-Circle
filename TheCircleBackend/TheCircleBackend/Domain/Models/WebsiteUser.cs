namespace TheCircleBackend.Domain.Models
{
    public class WebsiteUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public bool IsOnline { get; set; }
    }
}
