using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;
using TheCircleBackend.Domain.Models;
using TheCircleBackend.DomainServices.IRepo;

namespace TheCircleBackend.Hubs
{
    public class ViewerHub: Hub
    {
        private readonly IViewerRepository viewerRepository;

        public ViewerHub(IViewerRepository viewerRepository)
        {
            this.viewerRepository = viewerRepository;
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        // TODO Methods need to be tested. Perhaps a seperate stream.
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            //Delete viewer
            viewerRepository.RemoveViewer(Context.ConnectionId);

            int watchCount = viewerRepository.RemoveViewer(Context.ConnectionId);

            Clients.All.SendAsync("UpdateViewerCount", watchCount);

            return base.OnDisconnectedAsync(exception);
        }

        public async Task ConnectToStream(Viewer viewer)
        {
            //Appends ConnectionId to Viewer
            viewer.ConnectionId = Context.ConnectionId;

            //Adds viewer to database
            viewerRepository.Create(viewer);

            int watchCount = viewerRepository.GetViewershipCount(viewer.StreamId);

            await Clients.All.SendAsync("UpdateViewerCount", watchCount);
        }
    }
}
