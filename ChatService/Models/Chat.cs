using System;

namespace ChatService.Models
{
    public class Chat
    {
        public Guid TripId { get; set; }
        public Guid DriverId { get; set; }
        public Guid PassengerId { get; set; }
        public DateTime TripCreatedTime { get; set; }
    }
}
