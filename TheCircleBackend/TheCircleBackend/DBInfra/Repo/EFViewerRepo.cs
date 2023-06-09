using Microsoft.EntityFrameworkCore;
using TheCircleBackend.Domain.Models;
using TheCircleBackend.DomainServices.IRepo;

namespace TheCircleBackend.DBInfra.Repo
{
    public class EFViewerRepo : IViewerRepository
    {
        private readonly DomainContext domainContext;

        public EFViewerRepo(DomainContext domainContext)
        {
            this.domainContext = domainContext;
        }

        public bool Create(Viewer entity)
        {
            try
            {
                domainContext.Add(entity);
                domainContext.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public Viewer Delete(int id)
        {
            throw new NotImplementedException();
        }

        public List<Viewer> GetAll(int? id)
        {
            throw new NotImplementedException();
        }

        public Viewer GetById(int id)
        {
            throw new NotImplementedException();
        }

        public int GetCurrentViewerCount(int userId)
        {
            throw new NotImplementedException();
        }

        public int GetViewershipCount(int streamId)
        {
            return domainContext.Viewer.Where(s => s.StreamId.Equals(streamId)).Count();
        }

        public int RemoveViewer(string connectionId)
        {
            var viewer = domainContext.Viewer.FirstOrDefault(u => u.ConnectionId.Equals(connectionId));
            if(viewer == null)
            {
                Console.WriteLine("Nothing to remove");
                return 0;
            }
            else
            {
                domainContext.Viewer.Remove(viewer);
                domainContext.SaveChanges();
                return 1;
            }
        }

        public Viewer Update(Viewer entity)
        {
            throw new NotImplementedException();
        }
    }
}
