using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;
using TheCircleBackend.Domain.Interfaces;

namespace TheCircleBackend.Domain.Models
{
    public class Stream: IDomain
    {
        public int UserId { get; init; }
        public string Title { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }

        public List<Viewer> ViewList { get; set; }
        public IList<SeeChangeStreamChunk> Chunks { get; init; } = new List<SeeChangeStreamChunk>();


        [Column(TypeName = "varbinary(MAX)")]
        public byte[] StreamVid { get; set; }

        public void AddChunk(SeeChangeStreamChunk chunk)
        {
            Chunks.Add(chunk);
        }
    }
}