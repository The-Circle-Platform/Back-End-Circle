using TheCircleBackend.Domain.Models;

namespace TheCircleBackend.DomainServices.IRepo
{
    public interface IVodRepo
    {
        public IEnumerable<Vod> GetVods();
        public Vod GetVodById(int id);
        public void AddVod(Vod vod);
    }
}
