using TheCircleBackend.Domain.Interfaces;
using TheCircleBackend.Domain.Models;
using Stream = TheCircleBackend.Domain.Models.Stream;

namespace TheCircleBackend.DomainServices.IRepo
{
    public interface IStreamRepository: IRepository<Stream>
    {
        public void UpdateStream(Stream stream, int streamId);

        public Stream GetById(int streamId);

        public bool Create(Stream stream);
    }
}