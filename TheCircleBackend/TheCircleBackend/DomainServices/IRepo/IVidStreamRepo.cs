using TheCircleBackend.Domain.Interfaces;

namespace TheCircleBackend.DomainServices.IRepo
{
    public interface IVidStreamRepo : IRepository<Domain.Models.Stream>
    {
        public bool StartStream(int UserId, string title);
        public bool StopStream(int UserId, int StreamId);


    }
}
