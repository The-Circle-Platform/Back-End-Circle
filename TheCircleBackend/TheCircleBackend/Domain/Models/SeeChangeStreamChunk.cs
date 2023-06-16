using TheCircleBackend.Domain.Interfaces;

namespace TheCircleBackend.Domain.Models
{
    public class SeeChangeStreamChunk : IDomain
    {
        public SeeChangeStreamChunk()
        {

        }
        public Guid StreamId { get; set; }
        public long SequenceNr { get; set; }
        public Guid UserId { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
        public int ChunkSize { get; set; }
        public byte[] Chunk { get; set; }
        public byte[] Hash { get; set; }
        public byte[] Signature { get; set; }

        public override string ToString()
        {
            return $"StreamId: {StreamId}, SequenceNr: {SequenceNr}, UserId: {UserId}, TimeStamp: {TimeStamp}, ChunkSize: {ChunkSize}, Chunk: {Chunk}, Hash: {Hash}, Signature: {Signature}";
        }
    }
}
