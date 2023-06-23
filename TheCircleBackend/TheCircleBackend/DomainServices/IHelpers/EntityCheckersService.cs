using TheCircleBackend.DomainServices.IRepo;

namespace TheCircleBackend.DomainServices.IHelpers
{
    public class EntityCheckersService : IEntityCheckerService
    {
        private readonly IWebsiteUserRepo websiteUserRepo;
        private readonly IVidStreamRepo vidStreamRepo;

        public EntityCheckersService(IWebsiteUserRepo websiteUserRepo, IVidStreamRepo vidStreamRepo)
        {
            this.websiteUserRepo = websiteUserRepo;
            this.vidStreamRepo = vidStreamRepo;
        }

        public bool StreamExists(int streamId)
        {
            return vidStreamRepo.GetById(streamId) != null;
        }

        public bool UserExists(int userId)
        {
            return websiteUserRepo.GetById(userId) != null;
        }
    }
}
