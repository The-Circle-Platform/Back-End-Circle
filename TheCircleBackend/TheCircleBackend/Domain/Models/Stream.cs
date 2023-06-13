using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;
using TheCircleBackend.Domain.Interfaces;

namespace TheCircleBackend.Domain.Models
{
    public class Stream: IDomain
    {
        public string Title { get; set; }
        
        public List<Viewer> ViewList { get; set; }


        [Column(TypeName = "varbinary(MAX)")]
        public byte[] StreamVid { get; set; }
    }
}