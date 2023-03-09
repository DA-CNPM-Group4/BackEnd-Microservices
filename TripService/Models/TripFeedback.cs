using System;

namespace TripService.Models
{
    public class TripFeedback
    {
        public Guid TripId { get; set; }
        public string Note { get; set; }
        public double Rate { get; set; }
    }
}
