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


            public Stream Delete(int id)
        {
            throw new NotImplementedException();
        }

        public List<Stream> GetAll(int? id)
        {
            throw new NotImplementedException();
        }

        public Stream GetById(int id)
        {
            this.logger.LogInformation("Get user with id {0}", id);
            return domainContext.Stream.Where(u => u.Id == id).FirstOrDefault();
        }

        public void Update(Stream stream, int streamId)
        {
            var result = domainContext.Stream.Where(u => u.Id == streamId).FirstOrDefault();
            Console.WriteLine(result.Title);
            if (result != null)
            {
                result.Title = stream.Title;
                result.StreamVid = stream.StreamVid;
                domainContext.SaveChanges();
            }
        }

        public Stream Update(Stream entity)
        {
            throw new NotImplementedException();
        }
    }
}
