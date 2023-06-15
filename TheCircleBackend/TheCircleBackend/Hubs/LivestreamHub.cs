using System;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Channels;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;


namespace TheCircleBackend.Hubs
{
    public class LivestreamHub : Hub
    {
        private readonly string _id;
        private string[] record;

        public LivestreamHub()
        {
            _id = Guid.NewGuid().ToString();
        }

        public string Call() => _id;


        public async Task Upload(Test test)
        {
            record.Append<string>(test.stream);
            Console.WriteLine("incoming:");
            Console.WriteLine(test.name + " " + test.stream);
            return Clients.All.SendAsync("ImageMessage", test.stream);

        }

        public Task LivestreamForClient(Test test)
        {
            return Clients.All.SendAsync("ImageMessage", test.stream);
        }
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

