using TheCircleBackend.Domain.Interfaces;
using TheCircleBackend.Domain.Models;

namespace TheCircleBackend.Domain.DTO
{
    public class ViewerIncomingContentDTO: IContent
    {
        public Viewer OriginalViewer { get; set; }
    }

    public class ViewerOutcomingContentDTO : IContent
    {
        public int OriginalCount { get; set; }
        public string ServerPublicKey { get; set; }
    }
}
