using TheCircleBackend.Domain.Interfaces;
using TheCircleBackend.Domain.Models;

namespace TheCircleBackend.DomainServices.IRepo
{
    public interface IStreamChunkRepo : IRepository<Streamchunks>
    {
        bool UpdateOrCreate(Streamchunks streamchunks);
    }
}
