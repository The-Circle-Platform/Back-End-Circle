using Microsoft.AspNetCore.SignalR;
using TheCircleBackend.Domain.DTO;
using TheCircleBackend.Domain.DTO.EncryptedPayload;
using TheCircleBackend.Domain.Models;
using TheCircleBackend.DomainServices.IHelpers;
using TheCircleBackend.DomainServices.IRepo;
using TheCircleBackend.Helper;

namespace TheCircleBackend.Hubs
{
    public class ChatHub: Hub
    {
        
        private readonly IChatMessageRepository messageRepository;
        private readonly ISecurityService security;
        private readonly ILogItemRepo logItemRepo;
        private readonly LogHelper logHelper;


        public ChatHub(IChatMessageRepository messageRepository, ILogItemRepo logItemRepo, ILogger<ChatHub> logger, ISecurityService security)
        {
            this.messageRepository = messageRepository;
            this.security = security;
            this.logHelper = new LogHelper(logItemRepo, logger, "ChatHub");
        }

        // Receiver method
        public async Task RetrieveCurrentChat(int receiverUserId)
        {
            //Retrieve current chat list
            List<ChatMessage> list = messageRepository.GetStreamChat(receiverUserId);

            // Generate server keypair
            var ServerKeyPair = security.GenerateKeys();

            // Creates hash and creates signature, based on this hash.
            var signedData = security.EncryptData(list, ServerKeyPair.privKey);

            // Creates DTO to send to client.
            var ChatMessageDTO = new OutComingChatContent()
            {
                OriginalContent = new ChatMessageDTOOutcoming()
                {
                    ReceiverId = receiverUserId,
                    Messages = list
                },
                PublicKey = ServerKeyPair.pubKey,
                Signature = signedData
            };

            //Send back to client.
            await Clients.All.SendAsync($"ReceiveChat-{receiverUserId}", ChatMessageDTO);
        }
        
        // Receiver method
        public async Task SendMessage(IncomingChatContent incomingChatMessage)
        {
            // RetrieveUserKeys
            var publicKeyUser = security.GetKeys(incomingChatMessage.SenderUserId).pubKey;

            // Checks if integrity is held.
            bool HeldIntegrity = security.HoldsIntegrity(incomingChatMessage.OriginalContent, incomingChatMessage.Signature, publicKeyUser);

            if (HeldIntegrity)
            {
                throw new ArgumentException("Message has been tampered. Try later");
            }
            else
            {
                // Persisteer in database. Tabel chats (StreamId, UserId, DatumTijd en Content)

                ChatMessage chatMessage = incomingChatMessage.OriginalContent.Message;

                //Inserts chat message
                messageRepository.Create(chatMessage);

                // Lees geupdate versie
                var updatedList = messageRepository.GetStreamChat(incomingChatMessage.OriginalContent.ReceiverId);

                // Generate server keypair
                var ServerKeyPair = security.GenerateKeys();

                // Creates hash and creates signature, based on this hash.
                // CreateDTO
                var ChatMessageDTO = new ChatMessageDTOOutcoming()
                {
                    ReceiverId = incomingChatMessage.OriginalContent.ReceiverId,
                    Messages = updatedList
                };

                var signedData = security.EncryptData(ChatMessageDTO, ServerKeyPair.privKey);

                // Creates DTO to send to client.
                var OutcomingMessage = new OutComingChatContent()
                {
                    PublicKey = ServerKeyPair.pubKey,
                    Signature = signedData,
                    OriginalContent = ChatMessageDTO,
                    SenderUserId = incomingChatMessage.SenderUserId
                };

                // Send new data to client.
                await Clients.All.SendAsync($"ReceiveChat-{incomingChatMessage.OriginalContent.ReceiverId}",
                    OutcomingMessage);
            }
        }

        public override Task OnConnectedAsync()
        {
            string connectionId = Context.ConnectionId;
            Console.WriteLine(connectionId);
            Console.WriteLine(Context.UserIdentifier);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine("Connectie verbroken!");
            return base.OnDisconnectedAsync(exception);
        }
    }
}
