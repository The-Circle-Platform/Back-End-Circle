using TheCircleBackend.Domain.Interfaces;

namespace TheCircleBackend.Domain.Models
{
    public class Streamchunks : IDomain
    {

        //Foreign Key relation to stream
        public int StreamId { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
        public int ChunkSize { get; set; }
        public string Chunk { get; set; }

        public Domain.Models.Stream RelatedStream { get; set; }

        public override string ToString()
        {
            return $"StreamId: {StreamId}, TimeStamp: {TimeStamp}, ChunkSize: {ChunkSize}, Chunk: {Chunk}";
        }
    }
}