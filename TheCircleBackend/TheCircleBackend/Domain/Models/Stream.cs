using TheCircleBackend.Domain.Interfaces;

namespace TheCircleBackend.Domain.Models
{
    public class Stream: IDomain
    {
        public string Title { get; set; }
        
        public List<Viewer> ViewList { get; set; }
    }
}
