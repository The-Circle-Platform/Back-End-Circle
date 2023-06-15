using System;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;


namespace TheCircleBackend.Hubs
{
    public class LivestreamHub : Hub
    {
        private readonly string _id;

        public LivestreamHub()
        {
            _id = Guid.NewGuid().ToString();
        }

        public string Call() => _id;


        public async Task Upload(IAsyncEnumerable<Data> dataStream)
        {
            await foreach (var data in dataStream)
            {
                Console.WriteLine("Received Data: {0},{1},{2}", data.Name, data.Stream , _id);
            }
        }
    }
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

