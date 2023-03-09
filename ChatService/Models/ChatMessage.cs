using System;

namespace ChatService.Models
{
    public class ChatMessage
    {
        public Guid TripId { get; set; }
        public string Message { get; set; }
        public string SenderName { get; set; }
        public DateTime SendTime { get; set; }
        public Guid SenderId { get; set; }
    }
}
