using System;

namespace InfoService.Models
{
    public class Staff
    {
        public Guid AccountId { get; set; }
        public string IdentityNumber { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public bool Gender { get; set; }
        public string Address { get; set; }
    }
}
