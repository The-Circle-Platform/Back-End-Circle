using Microsoft.AspNetCore.SignalR;
using TheCircleBackend.Domain.Models;
using TheCircleBackend.DomainServices.IRepo;

namespace TheCircleBackend.Hubs
{
    public class ChatHub: Hub
    {
        
        private readonly IChatMessageRepository messageRepository;

        public ChatHub(IChatMessageRepository messageRepository)
        {
            this.messageRepository = messageRepository;
        }


        public async Task RetrieveCurrentChat(int receiverUserId)
        {
            //Retrieve current chat list
            List<ChatMessage> list = messageRepository.GetStreamChat(receiverUserId);

            //Send back to client.
            await Clients.All.SendAsync($"ReceiveChat-{receiverUserId}", list);
        }

        public async Task SendMessage(ChatMessage incomingChatMessage)
        {

            Console.WriteLine(Context.ConnectionId);
            // Decrypt message, HIER MOET NOG EEN METHODE KOMEN.

            // Persisteer in database. Tabel chats (StreamId, UserId, DatumTijd en Content)
            messageRepository.Create(incomingChatMessage);
            // Lees geupdate versie
            var updatedList = messageRepository.GetStreamChat(incomingChatMessage.ReceiverId);
            
            // Encrypt the message
            
            Console.WriteLine("Nieuwe lijst");
            // Send new data to client. HIER MOET NOG EEN METHODE KOMEN.
            await Clients.All.SendAsync($"ReceiveChat-{incomingChatMessage.ReceiverId}", updatedList);
        }

        public override Task OnConnectedAsync()
        {
            string connectionId = Context.ConnectionId;
            Console.WriteLine(connectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine("Connectie verbroken!");
            Console.WriteLine(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
