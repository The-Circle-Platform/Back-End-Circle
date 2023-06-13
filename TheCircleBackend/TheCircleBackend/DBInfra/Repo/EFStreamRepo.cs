using TheCircleBackend.Domain.Models;
using TheCircleBackend.DomainServices.IRepo;
using Stream = TheCircleBackend.Domain.Models.Stream;

namespace TheCircleBackend.DBInfra.Repo
{
    public class EFStreamRepo : IStreamRepository
    {
        private readonly DomainContext domainContext;
        private readonly ILogger<EFStreamRepo> logger;

        public EFStreamRepo(DomainContext domainContext)
        {
            this.domainContext = domainContext;
        }

        public bool Create(Stream entity)
        {
            try
            {
                this.domainContext.Add(entity);
                var result = this.domainContext.SaveChanges();

                return true;
            }
            catch
            {
                throw new Exception();
            }
        }


            public Domain.Models.Stream Delete(int id)
        {
            throw new NotImplementedException();
        }

        public List<Domain.Models.Stream> GetAll(int? id)
        {
            throw new NotImplementedException();
        }

        public Stream GetById(int id)
        {
            this.logger.LogInformation("Get user with id {0}", id);
            return domainContext.Stream.Where(u => u.Id == id).FirstOrDefault();
        }

        public Domain.Models.Stream Update(Domain.Models.Stream entity)
        {
            throw new NotImplementedException();
        }
    }
}
