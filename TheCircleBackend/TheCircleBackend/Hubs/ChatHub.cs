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
            this.logHelper = new LogHelper(logItemRepo, logger);
        }

        // Receiver method
        public async Task RetrieveCurrentChat(int receiverUserId)
        {
            //Retrieve current chat list
            List<ChatMessage> list = messageRepository.GetStreamChat(receiverUserId);

            // Generate server keypair
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
                var incom = incomingChatMessage;
                // RetrieveUserKeys
                var publicKeyUser = security.GetKeys(incomingChatMessage.SenderUserId).pubKey;

                // Checks if integrity is held.
                bool HeldIntegrity = security.HoldsIntegrity(incomingChatMessage.OriginalData, incomingChatMessage.Signature, publicKeyUser);

                if (HeldIntegrity)
                {
                    throw new ArgumentException("Message has been tampered. Try later");
                }
                else
                {
                    // Persisteer in database. Tabel chats (StreamId, UserId, DatumTijd en Content)

                    ChatMessage chatMessage = incomingChatMessage.OriginalData;

                    //Inserts chat message
                    messageRepository.Create(chatMessage);

                    // Lees geupdate versie
                    var updatedList = messageRepository.GetStreamChat(incomingChatMessage.OriginalData.ReceiverId);
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
                    // Generate server keypair
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
