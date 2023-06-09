﻿using TheCircleBackend.Domain.Interfaces;

namespace TheCircleBackend.Domain.DTO
{
    public class VideoStreamDTO
    {
        public int id { get; set; }
        public string? title { get; set; }
        public DateTime startStream { get; set; }
        public DateTime? endStream { get; set; }
        public string transparantUserName { get; set; }
        public int transparantUserId { get; set; }
    }

    public class NodeStreamOutput : IContent
    {
        public bool OriginalData { get; set; }
        public string message { get; set; }
    }

    public class NodeStreamInputDTO : IContent
    {
        public NodeStreamInput OriginalData { get; set; }
    }

    public class NodeStreamInput
    {
        public string UserName { get; set; }
        public long TimeStamp { get; set; }

    }

    public class StreamDTO
    {
        public NodeStreamInput OriginalData { get; set; }
        public string signature { get; set; }
    }
}
