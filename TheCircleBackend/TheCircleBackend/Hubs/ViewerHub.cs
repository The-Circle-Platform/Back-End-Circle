using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;
using TheCircleBackend.Domain.Models;
using TheCircleBackend.DomainServices.IRepo;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace TheCircleBackend.Hubs
{
    public class ViewerHub: Hub
    {
        private readonly IViewerRepository viewerRepository;

        public ViewerHub(IViewerRepository viewerRepository)
        {
            this.viewerRepository = viewerRepository;
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

        public async Task ConnectToStream(Viewer viewer)
        {
            Console.WriteLine("Connection made");
            // TODO Needs security check.

            // It needs to check if user has not more than 4 streams open.
            bool IsAllowed = CheckMaxViews(viewer.UserId);

            //Appends ConnectionId to Viewer
            viewer.ConnectionId = Context.ConnectionId;

            if (!IsAllowed)
            {
                //It will send back false and user is not allowed to watch the stream.
                AllowUserToWatch(IsAllowed, viewer.ConnectionId);
            }
            else
            {
                //Adds viewer to database
                viewerRepository.Create(viewer);

                // Updates new viewership count
                await UpdateViewCount(viewer.StreamId);
            }
        }

        private async Task UpdateViewCount(int streamId)
        {
            int watchCount = viewerRepository.GetViewershipCount(streamId);
            Console.WriteLine("Count is " + watchCount);
            await Clients.All.SendAsync($"UpdateViewerCount{streamId}", watchCount);
        }

        private async Task AllowUserToWatch(bool isAllowed, string ConnectId)
        {
            await Clients.Client(ConnectId).SendAsync("IsAllowed", isAllowed);
        }

        private bool CheckMaxViews(int watcherId)
        {
            return viewerRepository.GetCurrentViewerCount(watcherId) < 4;
        }
    }
}
