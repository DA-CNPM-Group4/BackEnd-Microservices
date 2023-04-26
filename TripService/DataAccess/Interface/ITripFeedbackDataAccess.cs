using TripService.Models;

namespace TripService.DataAccess.Interface
{
    public interface ITripFeedbackDataAccess
    {
        public Task<int> RateTrip(string userId, Guid tripId, string description, double rate);
        public Task<TripFeedback> GetTripFeedback(string userId, Guid tripId);
        public Task<int> ClearTable();
    }
}
