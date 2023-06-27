using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TheCircleBackend.Domain.Interfaces;

namespace TheCircleBackend.DomainServices.IRepo
{
    public interface IVidStreamRepo : IRepository<Domain.Models.Stream>
    {
        public bool StartStream(int UserId, string title, string streamKey);
        public bool StopStream(int UserId, int StreamId);
        public Domain.Models.Stream GetCurrentStream(int HostId);

    }
}
