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
using static System.Net.Mime.MediaTypeNames;

namespace TheCircleBackend.Hubs
{
    public class LivestreamHub : Hub
    {
        private readonly string _id;
        private readonly ISecurityService securityService;
        private string[] record;
        public string ClientEndpoint { get; private set; }
        public LivestreamHub(ISecurityService securityService)
        {
            _id = Guid.NewGuid().ToString();
            this.ClientEndpoint = "Stream-";
            this.securityService = securityService;
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

            // Creeer signature
            var SignatureOut = securityService.SignData(Original, ServerKeyPair.privKey);

            dto.Signature = SignatureOut;
            dto.OriginalData = Original;
            //record.Append<string>(test.stream);
            //Console.WriteLine("incoming:");
            //Console.WriteLine(test.name + " " + test.stream);
            //Console.WriteLine(test.stream);

            return Clients.All.SendAsync(this.ClientEndpoint + dto.OriginalData.streamId, ActualStreamChunk);

        }

        //public Task LivestreamHub(Test test)
        //{

        //}
    }
}

public class Test
{
    public string name { get; set; }
    public string stream { get; set; }
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
