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


        public async Task RetrieveCurrentChat(int streamId)
        {
            //Retrieve current chat list
            List<ChatMessage> list = messageRepository.GetStreamChat(streamId);

            //Send back to client.
            await Clients.All.SendAsync("ReceiveChat", list);
        }

        public async Task SendMessage(ChatMessage incomingChatMessage)
        {
  
            // Decrypt message, HIER MOET NOG EEN METHODE KOMEN.

            // Persisteer in database. Tabel chats (StreamId, UserId, DatumTijd en Content)
            messageRepository.Create(incomingChatMessage);
            // Lees geupdate versie
            var updatedList = messageRepository.GetStreamChat(incomingChatMessage.StreamId);
            
            // Encrypt the message
            
            Console.WriteLine("Nieuwe lijst");
            // Send new data to client. HIER MOET NOG EEN METHODE KOMEN.
            await Clients.All.SendAsync("ReceiveChat", updatedList);
        }

    }
}
