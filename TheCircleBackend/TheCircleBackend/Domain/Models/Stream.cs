using Newtonsoft.Json;
using TheCircleBackend.Domain.Interfaces;

namespace TheCircleBackend.Domain.Models
{
    public class Stream: IDomain
    {
        public string Title { get; set; }
        public DateTime StartStream { get; set; } = DateTime.Now;
        public DateTime? EndStream { get; set; } = null;
        public int StreamUserId { get; set; }
        
        public WebsiteUser User { get; set; }

        public List<Viewer> ViewList { get; set; }

        [JsonIgnore]
        public List<Streamchunks> RelatedStreamChunks { get; set; }
    }
}
