using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;
using TheCircleBackend.Domain.DTO;
using TheCircleBackend.Domain.Models;
using TheCircleBackend.DomainServices.IHelpers;
using TheCircleBackend.DomainServices.IRepo;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace TheCircleBackend.Hubs
{
    public class ViewerHub: Hub
    {
        private readonly IViewerRepository viewerRepository;
        private readonly ISecurityService securityService;

        public ViewerHub(IViewerRepository viewerRepository, ISecurityService securityService)
        {
            this.viewerRepository = viewerRepository;
            this.securityService = securityService;
        }

        // TODO Methods need to be tested. Perhaps a seperate stream.
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine("Disconnection made");
            
            String connectId = Context.ConnectionId;
            Console.WriteLine(connectId);
            //Get viewers
            Viewer? viewer = viewerRepository.GetStreamOfViewers(connectId);

            //If user has been connected, it will not be null.
            if (viewer != null)
            {
                //Couples StreamId of a user
                int StreamId = viewer.StreamId;
                //Delete viewer
                viewerRepository.RemoveViewer(connectId);

                // Retrieve updated viewcount
                UpdateViewCount(StreamId);
            }
            
            //Deconnects connection
            return base.OnDisconnectedAsync(exception);
        }

        public async Task ConnectToStream(ViewerIncomingContentDTO content)
        {
            Console.WriteLine("Connection made");
            // Retrieves public key of user.
            var publicKeyUser = securityService.GetKeys(content.OriginalViewer.UserId).pubKey;
            // Checks original data with signature, in order to control the data on data-integrity;
            bool HoldsIntegrity = securityService.HoldsIntegrity(content.OriginalViewer, content.Signature, publicKeyUser);
            
            if (!HoldsIntegrity)
            {
                throw new Exception("Integrity is tainted.");
            }

            var Viewer = content.OriginalViewer;
            
            // It needs to check if user has not more than 4 streams open.
            bool IsAllowed = CheckMaxViews(Viewer.UserId);

            //Appends ConnectionId to Viewer
            Viewer.ConnectionId = Context.ConnectionId;

            if (!IsAllowed)
            {
                //It will send back false and user is not allowed to watch the stream.
                AllowUserNotToWatch(IsAllowed, Viewer.ConnectionId);
            }
            else
            {
                //Adds viewer to database
                viewerRepository.Create(Viewer);

                // Updates new viewership count
                await UpdateViewCount(Viewer.StreamId);
            }
        }

        private async Task UpdateViewCount(int streamId)
        {
            int watchCount = viewerRepository.GetViewershipCount(streamId);
            Console.WriteLine("Count is " + watchCount);

            // Generate keypair
            var keyPair = securityService.GenerateKeys();
            //Signature
            var ServerSignature = securityService.EncryptData(streamId, keyPair.privKey);

            var ViewerContentOut = new ViewerOutcomingContentDTO()
            {
                Signature = ServerSignature,
                OriginalCount = watchCount,
                ServerPublicKey = keyPair.pubKey
            };
            await Clients.All.SendAsync($"UpdateViewerCount{streamId}", ViewerContentOut);
        }

        private async Task AllowUserNotToWatch(bool isAllowed, string ConnectId)
        {
            await Clients.Client(ConnectId).SendAsync("IsNotAllowed", isAllowed);
        }

        private bool CheckMaxViews(int watcherId)
        {
            return viewerRepository.GetCurrentViewerCount(watcherId) < 4;
        }
    }
}
