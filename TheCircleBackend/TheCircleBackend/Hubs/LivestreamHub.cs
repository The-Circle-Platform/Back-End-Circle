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

        public Task Upload(StreamChunkDTO dto)
        {
            var keyPair = securityService.GetKeys(dto.SenderUserId);

            var isValid = securityService.HoldsIntegrity(dto.OriginalData, dto.Signature, keyPair.pubKey);
            var ServerKeyPair = securityService.GetServerKeys();
            if (isValid)
            {
                
                var ErrorChunk = new StreamError()
                {
                    Error = true,
                    Message = "Data integriteit is aantetast"
                };
                //Signature
                var Signature = securityService.SignData(ErrorChunk, ServerKeyPair.privKey);

                var errorDto = new StreamErrorChunkDTO()
                {
                    Signature = Signature,
                    SenderUserId = dto.SenderUserId,
                    Error = ErrorChunk
                };
                
                return Clients.All.SendAsync(this.ClientEndpoint + "Error-" + dto.OriginalData.streamId, errorDto);
            }
            var Original = dto.OriginalData;
            //Zet dto om naar echte object
            var ActualStreamChunk = new Streamchunks()
            {
                StreamId = Original.streamId,
                Id = Original.id,
                Chunk = Original.chunk,
                ChunkSize = Original.chunkSize,
                TimeStamp = Original.timeStamp
            };

            //sla chunk op in database
            StreamChunkRepo.Create(ActualStreamChunk);
            // Creeer signature
            var SignatureOut = securityService.SignData(Original, ServerKeyPair.privKey);

            dto.Signature = SignatureOut;
            dto.OriginalData = Original;
            //record.Append<string>(test.stream);
            //Console.WriteLine("incoming:");
            //Console.WriteLine(test.name + " " + test.stream);
            //Console.WriteLine(test.stream);

            return Clients.All.SendAsync(this.ClientEndpoint + dto.OriginalData.streamId, dto);

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
