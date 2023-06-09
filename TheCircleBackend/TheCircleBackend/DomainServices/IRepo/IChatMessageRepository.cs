using TheCircleBackend.Domain.Interfaces;
using TheCircleBackend.Domain.Models;

namespace TheCircleBackend.DomainServices.IRepo
{
    public interface IChatMessageRepository: IRepository<ChatMessage>
    {
        public List<ChatMessage> GetByDate(DateTime date);
        public List<ChatMessage> GetStreamChat(int streamId);
    }
}
