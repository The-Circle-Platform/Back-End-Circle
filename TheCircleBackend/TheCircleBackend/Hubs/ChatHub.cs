using Microsoft.AspNetCore.SignalR;
using TheCircleBackend.Domain.DTO;
using TheCircleBackend.Domain.DTO.EncryptedPayload;
using TheCircleBackend.Domain.Models;
using TheCircleBackend.DomainServices.IHelpers;
using TheCircleBackend.DomainServices.IRepo;
using TheCircleBackend.Helper;

namespace TheCircleBackend.Hubs
{
    public class ChatHub : Hub
    {

        private readonly IChatMessageRepository messageRepository;
        private readonly IWebsiteUserRepo websiteUserRepo;
        private readonly IEntityCheckerService entityCheckerService;
        private readonly ISecurityService security;
        private readonly ILogItemRepo logItemRepo;
        private readonly LogHelper logHelper;


        public ChatHub(
            IChatMessageRepository messageRepository, 
            IWebsiteUserRepo websiteUserRepo,
            IEntityCheckerService entityCheckerService,
            ILogItemRepo logItemRepo, 
            ILogger<ChatHub> logger, 
            ISecurityService security)
        {
            this.messageRepository = messageRepository;
            this.websiteUserRepo = websiteUserRepo;
            this.entityCheckerService = entityCheckerService;
            this.security = security;
            this.logHelper = new LogHelper(logItemRepo, logger);
        }

        // Receiver method
        public async Task RetrieveCurrentChat(int receiverUserId)
        {
            if (!entityCheckerService.UserExists(receiverUserId))
            {
                throw new Exception("User does not exist");
            }
            //Retrieve current chat list
            List<ChatMessage> list = messageRepository.GetStreamChat(receiverUserId);

            // Generate server key pair
            var ServerKeyPair = security.GetServerKeys();
            // Converts code
            var NewUpdatedList = new List<ChatMessageLite>();
            foreach (ChatMessage message in list)
            {
                NewUpdatedList.Add(new ChatMessageLite()
                {
                    receiverUser = message.ReceiverUser.UserName,
                    writer = message.Writer.UserName,
                    date = message.Date,
                    message = message.Message,
                    receiverId = message.ReceiverId,
                    webUserId = message.WebUserId
                });
            }
            // Creates hash and creates signature, based on this hash.
            var signedData = security.SignData(NewUpdatedList, ServerKeyPair.privKey);

            // Creates DTO to send to client.
            var ChatMessageDTO = new ChatListDTOOutcoming()
            {
                OriginalList = NewUpdatedList,
                /*                PublicKey = ServerKeyPair.pubKey,*/
                Signature = signedData
            };

            //Send back to client.
            await Clients.All.SendAsync($"ReceiveChat-{receiverUserId}", ChatMessageDTO);
        }

        // Receiver method
        public async Task SendMessage(ChatMessageDTOIncoming incomingChatMessage)
        {
            //Checks if 1 minute has passed. If it is, it will trow error message.
            if ((DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - 60000) > incomingChatMessage.OriginalData.TimeSpan)
            {
                throw new TimeoutException("Timespan has expired");
            }

            bool UserAndReceiverExist = EntitiesExist(incomingChatMessage.OriginalData.WebUserId, incomingChatMessage.OriginalData.ReceiverId);
            
            if (!UserAndReceiverExist)
            {
                throw new ArgumentException("Entities do not exist");
            }

            var original = incomingChatMessage.OriginalData;
            var signature = incomingChatMessage.Signature;

            // RetrieveUserKeys
            var publicKeyUser = security.GetKeys(incomingChatMessage.SenderUserId);

            // Checks if integrity is held.
            bool HeldIntegrity = security.HoldsIntegrity(original, signature, publicKeyUser);

            if (!HeldIntegrity)
            {
                throw new ArgumentException("Message has been tampered. Try later");
            }
            else
            {
                //Persist in database: Table, chats (StreamId, UserId, DateTime and Content)
                ChatMessage chatMessage = new ChatMessage()
                {
                    Message = original.Message,
                    ReceiverId = original.ReceiverId,
                    WebUserId = original.WebUserId,
                    Date = original.Date,
                };

                //Inserts chat message
                messageRepository.Create(chatMessage);

                // Read updated version
                var updatedList = messageRepository.GetStreamChat(original.ReceiverId);
                // Converts code
                var NewUpdatedList = new List<ChatMessageLite>();
                foreach (ChatMessage message in updatedList)
                {
                    NewUpdatedList.Add(new ChatMessageLite()
                    {
                        receiverUser = message.ReceiverUser.UserName,
                        writer = message.Writer.UserName,
                        date = message.Date,
                        message = message.Message,
                        receiverId = message.ReceiverId,
                        webUserId = message.WebUserId
                    });
                }
                // Generate server key pair
                var ServerKeyPair = security.GetServerKeys();

                // Creates hash and creates signature, based on this hash.
                var signedData = security.SignData(NewUpdatedList, ServerKeyPair.privKey);

                // Creates DTO to send to client.
                var OutcomingMessage = new ChatListDTOOutcoming()
                {
                    Signature = signedData,
                    OriginalList = NewUpdatedList,
                    SenderUserId = incomingChatMessage.SenderUserId
                };

                // Send new data to client.
                await Clients.All.SendAsync($"ReceiveChat-{incomingChatMessage.OriginalData.ReceiverId}",
                    OutcomingMessage);
            }
        }

        private bool EntitiesExist(int UserId, int ReceiverId)
        {
            return entityCheckerService.UserExists(UserId) && entityCheckerService.UserExists(ReceiverId);
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine("Disconnected!");
            return base.OnDisconnectedAsync(exception);
        }
    }
}
