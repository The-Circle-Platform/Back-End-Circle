using System;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Channels;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using TheCircleBackend.Domain.DTO.EncryptedPayload;
using TheCircleBackend.Domain.Models;
using TheCircleBackend.DomainServices.IHelpers;
using TheCircleBackend.DomainServices.IRepo;
using static System.Net.Mime.MediaTypeNames;

namespace TheCircleBackend.Hubs
{
    public class LivestreamHub : Hub
    {
        private readonly string _id;
        private readonly ISecurityService securityService;
        private string[] record;
        public string ClientEndpoint { get; private set; }
        public IStreamChunkRepo StreamChunkRepo { get; }

        public LivestreamHub(ISecurityService securityService, IStreamChunkRepo streamChunkRepo)
        {
            _id = Guid.NewGuid().ToString();
            this.ClientEndpoint = "Stream-";
            this.securityService = securityService;
            StreamChunkRepo = streamChunkRepo;
        }

        public string Call() => _id;

        public async Task Upload(string chunk, int streamId, int HostId)
        {
            /*var ActualStreamChunk = new Streamchunks()
            {
                StreamId = streamId,
                Id = 0,
                Chunk = chunk,
                ChunkSize = chunk.Length,
                TimeStamp = new DateTimeOffset()
            };

             //sla chunk op in database
             StreamChunkRepo.Create(ActualStreamChunk);
             var ServerKeyPair = securityService.GetServerKeys();
             // Creeer signature
             var SignatureOut = securityService.SignData(ActualStreamChunk, ServerKeyPair.privKey);

             var dto = new
             {
                 Signature = SignatureOut,
                 OriginalData = ActualStreamChunk
             };*/
            Console.WriteLine("Endpoint Host: " + HostId);
            Console.WriteLine("Endpoint stream" + streamId);
            Console.WriteLine("Endpoint chunk" + chunk);
            await Clients.All.SendAsync("Stream-1", "Endpoint is aangekomen" + chunk + HostId);
        }

        //public Task LivestreamHub(Test test)
        //{

        //}
    }
}

public class Stream
{
    public int StreamId { get; set; }
    public DateTimeOffset TimeStamp { get; set; }
    public int ChunkSize { get; set; }
    public string Chunk { get; set; }
}

public class Data
{
    public string Name { get; set; }
    public Task<string>[] Stream { get; set; }

    public Data(string name, Task<string>[] stream)
    {
        Name = name;
        Stream = stream;
    }
}
