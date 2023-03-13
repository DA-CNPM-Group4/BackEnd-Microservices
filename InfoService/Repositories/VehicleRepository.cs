using InfoService.Models;
using Microsoft.EntityFrameworkCore;

namespace InfoService.Repositories
{
    public class VehicleRepository : BaseRepository
    {
        public VehicleRepository(InfoDbContext context) : base(context)
        {
        }
        public async Task<bool> CheckDriverHasVehicleAlready(Guid driverId)
        {
            return await context.Vehicle.AnyAsync(p => p.DriverId == driverId);
        }

        public async Task<int> RegisterVehicle(Vehicle vehicle)
        {
            vehicle.VehicleId = vehicle.DriverId;
            await context.Vehicle.AddAsync(vehicle);
            return await context.SaveChangesAsync();
        }

        public async Task<Vehicle> GetDriverVehicle(Guid driverId)
        {
            return await context.Vehicle.Where(p=>p.DriverId == driverId).SingleOrDefaultAsync();
        }
    }
}
