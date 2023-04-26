using TripService.Models;

namespace TripService.DataAccess.Interface
{
    public interface ITripDataAccess
    {
        public Task<List<Trip>> GetTrips();
        public Task<Guid> AcceptTrip(string userId, string driverId, string requestId);
        public Task<int> PickedPassenger(string userId, Guid tripId);
        public Task<Models.Trip> GetTripForPassenger(string userId, Guid passengerId, Guid requestId);
        public Task<List<Models.Trip>> GetListTripsByDriver(string userId, Guid driverId);
        public Task<List<Models.Trip>> GetListTripsByPassenger(string userId, Guid passengerId);
        public Task<int> CancelTrip(string userId, Guid tripId);
        public Task<Models.Trip> GetTrip(string userId, Guid tripId);
        public Task<object> GetCompletedTrips(string userId, Guid driverId, string from, string to);
        public Task<int> GetIncome(string userId, Guid driverId, string from, string to);
        public Task<int> CompleteTrip(string userId, Guid tripId);
        public Task<int> CalcNumOfPagesForPassenger(string userId, Guid passengerId, int pageSize);
        public Task<int> CalcNumOfPagesForDriver(string userId, Guid driverId, int pageSize);
        public Task<List<Models.Trip>> GetPassengerTripsPaging(string userId, Guid passengerId, int pageSize, int pageNum);
        public Task<List<Models.Trip>> GetDriverTripsPaging(string userId, Guid driverId, int pageSize, int pageNum);
        public Task<int> ClearTable();
    }
}
