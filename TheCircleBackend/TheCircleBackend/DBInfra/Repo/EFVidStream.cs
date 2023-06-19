using TheCircleBackend.DomainServices.IRepo;

namespace TheCircleBackend.DBInfra.Repo
{
    public class EFVidStream : IVidStreamRepo
    {
        private readonly DomainContext domainContext;

        public EFVidStream(DomainContext domainContext)
        {
            this.domainContext = domainContext;
        }

        public bool Create(Domain.Models.Stream entity)
        {
            throw new NotImplementedException();
        }

        public Domain.Models.Stream Delete(int id)
        {
            throw new NotImplementedException();
        }

        public List<Domain.Models.Stream> GetAll(int? id)
        {
            throw new NotImplementedException();
        }

        public Domain.Models.Stream? GetById(int id)
        {
            try
            {
                //Retrieves current stream
                return domainContext.VideoStream
                    .OrderByDescending(v => v.StartStream)
                    .FirstOrDefault(vs => vs.Id == id && vs.EndStream != null);
            }
            catch
            {
                throw new Exception("Stream vinden ging niet goed");
            }
        }

        public bool StartStream(int UserId, string title)
        {
            try {
                domainContext.VideoStream.Add(new Domain.Models.Stream()
                {
                    StreamUserId = UserId,
                    Title = title
                });
                domainContext.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool StopStream(int UserId, int StreamId)
        {
            try
            {
                var Stream = domainContext.VideoStream.First(vs => vs.Id == StreamId && vs.User.Id == UserId);
                Stream.EndStream = DateTime.Now;
                domainContext.VideoStream.Update(Stream);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Domain.Models.Stream Update(Domain.Models.Stream entity)
        {
            throw new NotImplementedException();
        }
    }
}
