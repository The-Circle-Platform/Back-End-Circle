using TheCircleBackend.Domain.Interfaces;
using TheCircleBackend.Domain.Models;
using Stream = TheCircleBackend.Domain.Models.Stream;

namespace TheCircleBackend.DomainServices.IRepo
{
    public interface IStreamRepository: IRepository<Stream>
    {
        
    }
}