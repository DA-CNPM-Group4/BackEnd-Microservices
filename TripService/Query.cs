using TripService.Models;
using TripService.Repositories;

namespace TripService
{
    public class Query
    {
        private readonly ServiceRepository _repository;
        public Query() { 
            _repository = new ServiceRepository();
        }
        public async Task<List<Trip>> GetAllTrips()
        {
            return await _repository.Trip.GetTrips();
        }

        public async Task<List<Trip>> GetTripByDriverId(string driverId)
        {
            return await _repository.Trip.GetListTripsByDriver(Guid.Parse(driverId));
        }

        public async Task<List<Trip>> GetTripByPassengerId(string passengerId)
        {
            return await _repository.Trip.GetListTripsByPassenger(Guid.Parse(passengerId));
        }

    }
}
