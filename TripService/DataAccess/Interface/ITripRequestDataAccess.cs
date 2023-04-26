using TripService.Models;

namespace TripService.DataAccess.Interface
{
    public interface ITripRequestDataAccess
    {
        public Task<int> CreateRequest(string userId, TripRequest request);
        public Task<int> CancelRequest(string userId, Guid requestId);
        public object CalcPrice(double distance);
        public Task<int> ClearTable();
    }
}
