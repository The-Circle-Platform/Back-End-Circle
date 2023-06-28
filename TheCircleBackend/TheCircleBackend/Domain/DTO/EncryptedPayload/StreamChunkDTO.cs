using TheCircleBackend.Domain.Interfaces;
using TheCircleBackend.Domain.Models;

namespace TheCircleBackend.Domain.DTO.EncryptedPayload
{
    public class StreamChunkInOutDTO
    {
        public int id { get; set; }
        //ForeignKey relatie naar stream
        public int streamId { get; set; }
        //public long SequenceNr { get; set; }
        public DateTimeOffset timeStamp { get; set; }
        public int chunkSize { get; set; }
        public string chunk { get; set; }
    }

    public class StreamChunkDTO: IContent
    {
        public StreamChunkInOutDTO OriginalData { get; set; }
    }



    public class StreamErrorChunkDTO : IContent
    {
        public StreamError Error { get; set; }
    }



    public class StreamError
    {
        public bool Error { get; set; }
        public string Message { get; set; }
    }
}
