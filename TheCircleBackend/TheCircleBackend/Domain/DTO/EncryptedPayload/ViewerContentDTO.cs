using TheCircleBackend.Domain.Interfaces;
using TheCircleBackend.Domain.Models;

namespace TheCircleBackend.Domain.DTO.EncryptedPayload
{
    public class ViewerIncomingContentDTO : IContent
    {
        public Viewer OriginalViewer { get; set; }
    }

    public class ViewerOutcomingContentDTO : IOutComingContent
    {
        public int OriginalCount { get; set; }
        public bool? OriginalAllowWatch { get; set; }
    }
}
