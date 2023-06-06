namespace TheCircleBackend.Domain.AuthModels
{
    public class AuthUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string HashPassword { get; set; }
        public bool IsValid { get; set; }
        public string PrivateKey { get; set; }

    }
}
