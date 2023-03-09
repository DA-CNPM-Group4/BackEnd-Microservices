using System;

namespace InfoService.Models
{
    public class Vehicle
    {
        public Guid DriverId { get; set; }
        public Guid VehicleId { get; set; }
        public string VehicleType { get; set; } // Khai bao danh muc cho value nay
        public string VehicleName { get; set; }
        public string Brand { get; set; }
    }
}
