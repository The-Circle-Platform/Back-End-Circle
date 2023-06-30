using Microsoft.EntityFrameworkCore.Infrastructure;
using TheCircleBackend.Domain.Models;
using TheCircleBackend.DomainServices.IHelpers;
using TheCircleBackend.DomainServices.IRepo;
using TheCircleBackend.Helper;

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

        public Domain.Models.Stream GetById(int StreamId)
        {
            try
            {
                //Retrieves current stream
                var list = domainContext
                    .VideoStream
                    .OrderByDescending(v => v.StartStream)
                    .ToList();

                var entity = list.First(v => v.Id == StreamId);

                return entity;
            }
            catch(Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public Domain.Models.Stream? GetCurrentStream(int HostId)
        {
            //Retrieves current stream
            var list = domainContext
                .VideoStream
                .OrderByDescending(v => v.StartStream)
                .ToList();

            var entity = list.Find(v => v.StreamUserId == HostId && v.EndStream == null);

            return entity;
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

                var User = domainContext.WebsiteUser.First(vs => vs.Id == UserId);
                User.IsOnline = false;
                domainContext.WebsiteUser.Update(User);

                ICoinHelper coinHelper = new CoinHelper(domainContext, UserId);
                coinHelper.ChangeBalance(Stream.StartStream, DateTime.Now);

                domainContext.SaveChanges();
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
