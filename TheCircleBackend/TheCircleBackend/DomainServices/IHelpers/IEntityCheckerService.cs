namespace TheCircleBackend.DomainServices.IHelpers
{
    public interface IEntityCheckerService
    {
        bool UserExists(int userId);
        bool StreamExists(int streamId);
    }
}
