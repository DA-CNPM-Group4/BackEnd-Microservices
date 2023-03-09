using System;

namespace InfoService.Models
{
    public class Driver
    {
        public Guid AccountId { get; set; }
        public string IdentityNumber { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public bool Gender { get; set; }
        public string Address { get; set; }
        public double AverageRate { get; set; }
        public int NumberOfRate { get; set; }
        public int NumberOfTrip { get; set; }
    }
}
