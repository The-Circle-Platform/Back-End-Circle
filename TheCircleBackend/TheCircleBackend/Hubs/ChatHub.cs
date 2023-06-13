using Microsoft.AspNetCore.SignalR;
using TheCircleBackend.Domain.DTO;
using TheCircleBackend.Domain.Models;
using TheCircleBackend.DomainServices.IHelpers;
using TheCircleBackend.DomainServices.IRepo;

namespace TheCircleBackend.Hubs
{
    public class ChatHub: Hub
    {
        
        private readonly IChatMessageRepository messageRepository;
        private readonly ISecurity security;

        public ChatHub(IChatMessageRepository messageRepository, ISecurity security)
        {
            this.messageRepository = messageRepository;
            this.security = security;
        }


        public async Task RetrieveCurrentChat(int receiverUserId)
        {
            //Retrieve current chat list
            List<ChatMessage> list = messageRepository.GetStreamChat(receiverUserId);

            //Send back to client.
            await Clients.All.SendAsync($"ReceiveChat-{receiverUserId}", list);
        }

        public async Task SendMessage(ChatMessageDTOIncomming incomingChatMessage)
        {

            Console.WriteLine(Context.ConnectionId);
            // Decrypt message, HIER MOET NOG EEN METHODE KOMEN.
            string publicKeyUser = "";
            bool HeldIntegrity = security.HoldsIntegrity(incomingChatMessage, incomingChatMessage.Signature, publicKeyUser);

            if (HeldIntegrity)
            {
                throw new ArgumentException("Message has been tampered. Try later");
            } 
            else
            {
                // Persisteer in database. Tabel chats (StreamId, UserId, DatumTijd en Content)
                ChatMessage chatMessage = incomingChatMessage.Messages;
                messageRepository.Create(chatMessage);
                // Lees geupdate versie
                var updatedList = messageRepository.GetStreamChat(incomingChatMessage.ReceiverId);

                // Encrypt the message

                // New signature
                var newSignature = security.CreateSignature();

                Console.WriteLine("Nieuwe lijst");
                // Send new data to client. HIER MOET NOG EEN METHODE KOMEN.
                await Clients.All.SendAsync($"ReceiveChat-{incomingChatMessage.ReceiverId}", updatedList);
            }
            
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
