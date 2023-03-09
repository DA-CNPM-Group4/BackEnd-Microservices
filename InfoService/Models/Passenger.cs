using System;

namespace InfoService.Models
{
    public class Passenger
    {
        public Guid AccountId { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public bool Gender { get; set; }
    }
}
