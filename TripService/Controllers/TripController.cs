using Helper.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TripService.Models;
using static Helper.Catalouge;

namespace TripService.Controllers
{
    [Route("api/Trip/[controller]/[action]")]
    [ApiController]
    public class TripController : BaseController
    {
        [HttpPost]
        public async Task<ResponseMsg> AcceptRequest(string driverId, string requestId)
        {
            Guid tripId = await Repository.Trip.AcceptTrip(driverId, requestId);
            return new ResponseMsg
            {
                status = tripId != Guid.Empty ? true : false,
                data = tripId,
                message = tripId != Guid.Empty ? "Accept request successfully" : "Failed to accept this request",
            };
        }

        [HttpPost]
        public async Task<ResponseMsg> FinishTrip(string tripId)
        {
            int result = await Repository.Trip.CompleteTrip(Guid.Parse(tripId));

            return new ResponseMsg
            {
                status = result > 0 ? true : false,
                data = null,
                message = result > 0 ? "Trip is finished" : "Failed to finish trip",
            };
        }

        [HttpGet]
        public async Task<ResponseMsg> CancelTrip(string tripId)
        {
            int result = await Repository.Trip.CancelTrip(Guid.Parse(tripId));
            return new ResponseMsg {
                status = result > 0 ? true : false,
                data = null,
                message = result > 0 ? "Cancel trip successfully" : "Failed to cancel this trip"
            };
            
        }

        [HttpGet]
        public async Task<ResponseMsg> GetPassengerTrips(string passengerId)
        {
            List<Models.Trip> trips = await Repository.Trip.GetListTripsByPassenger(Guid.Parse(passengerId));
            return new ResponseMsg
            {
                status = trips != null ? true : false,
                data = trips,
                message = trips != null ? "Get trips by passenger successfully" : "Failed to get trips by passenger"
            };
        }

        [HttpGet]
        public async Task<ResponseMsg> GetDriverTrips(string driverId)
        {
            List<Models.Trip> trips = await Repository.Trip.GetListTripsByDriver(Guid.Parse(driverId));
            return new ResponseMsg
            {
                status = trips != null ? true : false,
                data = trips,
                message = trips != null ? "Get trips by driver successfully" : "Failed to get trips by driver"
            };
        }

        [HttpGet]
        public async Task<ResponseMsg> GetCurrentTrip(string tripId)
        {
            Models.Trip result = await Repository.Trip.GetTrip(Guid.Parse(tripId));
            return new ResponseMsg
            {
                status = result != null ? true : false,
                data = result,
                message = result != null ? "Get trip successfully" : "Failed to get trip",
            };
        }



        [HttpGet]
        public async Task<ResponseMsg> GetCurrentTripForPassenger(string passengerId, string requestId)
        {
            Models.Trip trip = await Repository.Trip.GetTripForPassenger(Guid.Parse(passengerId), Guid.Parse(requestId));
            return new ResponseMsg
            {
                status = trip != null ? true : false,
                data = trip,
                message = trip != null ? "Get trip successfully" : "Failed to get trip",
            };
        }

        [HttpGet]
        public async Task<ResponseMsg> PickedPassenger(string tripId)
        {
            int result = await Repository.Trip.PickedPassenger(Guid.Parse(tripId));
            return new ResponseMsg
            {
                status = result > 0 ? true : false,
                data = result,
                message = result > 0 ? "Update status picked passenger successfully" : "Failed to update status picked passenger"
            };
        }

        [HttpGet]
        public async Task<ResponseMsg> GetTrips()
        {
            return new ResponseMsg
            {
                status = true,
                data = await Repository.Trip.GetTrips(),
                message = "Get trips successfully"
            };
        }
    }
}
