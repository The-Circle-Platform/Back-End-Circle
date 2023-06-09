﻿using TheCircleBackend.Domain.Interfaces;
using TheCircleBackend.Domain.Models;

namespace TheCircleBackend.DomainServices.IRepo
{
    public interface IViewerRepository: IRepository<Viewer>
    {

        public int GetCurrentViewerCount(int userId);
        public int RemoveViewer(string connectionId);
        public int GetViewershipCount(int streamId);
    }
}
