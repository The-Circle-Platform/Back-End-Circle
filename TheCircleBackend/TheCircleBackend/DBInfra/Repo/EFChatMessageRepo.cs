using Microsoft.EntityFrameworkCore;
using TheCircleBackend.Domain.Models;
using TheCircleBackend.DomainServices.IRepo;

namespace TheCircleBackend.DBInfra.Repo
{
    public class EFChatMessageRepo : IChatMessageRepository
    {
        private readonly DomainContext context;

        public EFChatMessageRepo(DomainContext domainContext)
        {
            this.context = domainContext;
        }

        public bool Create(ChatMessage entity)
        {
            try
            {
                this.context.Add(entity);
                var result = this.context.SaveChanges();
                
                return true;
            }
            catch
            {
                throw new Exception();
            }
            
        }

        public ChatMessage Delete(int id)
        {
            throw new NotImplementedException();
        }

        public List<ChatMessage> GetAll(int? id)
        {
            throw new NotImplementedException();
        }

        public List<ChatMessage> GetByDate(DateTime date)
        {
            throw new NotImplementedException();
        }

        public ChatMessage? GetById(int id)
        {
            try
            {
                return context.ChatMessage.SingleOrDefault(ch => ch.Id == id);
            }
            catch
            {
                throw new Exception("Database error");
            }
        }

        public List<ChatMessage> GetStreamChat(int transId)
        {
            try
            {
                return context.ChatMessage
                    .Include(wu => wu.Writer)
                    .Where(transUser => transUser.ReceiverId == transId)
                    .OrderBy(ch => ch.Date)
                    .ToList();
            }
            catch
            {
                throw new Exception("Error database");
            }
            
        }

        public ChatMessage Update(ChatMessage entity)
        {
            throw new NotImplementedException();
        }
    }
}
