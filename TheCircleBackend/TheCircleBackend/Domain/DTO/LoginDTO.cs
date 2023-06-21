using System.ComponentModel.DataAnnotations;

namespace TheCircleBackend.Domain.DTO
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Request is required")]
        public LoginDTORequest Request { get; set; }

        [Required(ErrorMessage = "Signature is required")]
        public string Signature { get; set; }
    }

    public class LoginDTORequest
    {
        public string UserName { get; set; }
        public long TimeStamp { get; set; }
    }

}
