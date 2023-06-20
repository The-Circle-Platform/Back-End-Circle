using System.Runtime.Intrinsics.Arm;
using TheCircleBackend.Domain.Interfaces;
using TheCircleBackend.Domain.Models;
using TheCircleBackend.DomainServices.IRepo;

namespace TheCircleBackend.DBInfra.Repo
{
    public class EFStreamChunkRepo : IStreamChunkRepo
    {
        public EFStreamChunkRepo(DomainContext domainContext) {
            DomainContext = domainContext;
        }

        public DomainContext DomainContext { get; }

        bool IRepository<Streamchunks>.Create(Streamchunks entity)
        {

            DomainContext.Streamchunks.Add(entity);
            DomainContext.SaveChanges();
            return true;
            /*
                    public int StreamId { get; set; }
                    public DateTimeOffset TimeStamp { get; set; }
                    public int ChunkSize { get; set; }
                    public string Chunk
                    {
                        get; set;
                    }*/
        }

        public bool UpdateOrCreate(Streamchunks streamchunks)
        {
            var IdCheck = DomainContext.Streamchunks.FirstOrDefault(a => a.StreamId== streamchunks.StreamId);
            if(IdCheck == null)
            {
                DomainContext.Streamchunks.Add(streamchunks);
                return true;
            }
            IdCheck.Chunk += streamchunks.Chunk;
            DomainContext.Streamchunks.Update(IdCheck);
            DomainContext.SaveChanges();
            return true;

        }

        Streamchunks IRepository<Streamchunks>.Delete(int id)
        {
            throw new NotImplementedException();
        }

        List<Streamchunks> IRepository<Streamchunks>.GetAll(int? id)
        {
            throw new NotImplementedException();
        }

        Streamchunks IRepository<Streamchunks>.GetById(int id)
        {
            throw new NotImplementedException();
        }

        Streamchunks IRepository<Streamchunks>.Update(Streamchunks entity)
        {
            throw new NotImplementedException();
        }
    }
}
