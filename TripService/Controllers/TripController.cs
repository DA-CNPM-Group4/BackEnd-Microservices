using Helper.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using TripService.DTOs;
using TripService.Models;
using static Helper.Catalouge;

namespace TripService.Controllers
{
    [Route("api/trip/[controller]/[action]")]
    [ApiController]
    public class TripController : BaseController
    {
        [HttpPost]
        public async Task<ResponseMsg> AcceptRequest(AcceptTripDTO acceptTripDTO)
        {
            Guid tripId = await Repository.Trip.AcceptTrip(acceptTripDTO.DriverId, acceptTripDTO.RequestId);
            return new ResponseMsg
            {
                status = tripId != Guid.Empty ? true : false,
                data = tripId,
                message = tripId != Guid.Empty ? "Accept request successfully" : "Failed to accept this request",
            };
        }

        [HttpPost]
        public async Task<ResponseMsg> FinishTrip([FromBody] object tripIdJson)
        {
            JObject objTemp = JObject.Parse(tripIdJson.ToString());
            string tripId = (string)objTemp["tripId"];
            int result = await Repository.Trip.CompleteTrip(Guid.Parse(tripId));

            return new ResponseMsg
            {
                status = result > 0 ? true : false,
                data = null,
                message = result > 0 ? "Trip is finished" : "Failed to finish trip",
            };
        }

        [HttpPost]
        public async Task<ResponseMsg> CancelTrip([FromBody] object tripIdJson)
        {
            JObject objTemp = JObject.Parse(tripIdJson.ToString());
            string tripId = (string)objTemp["tripId"];
            int result = await Repository.Trip.CancelTrip(Guid.Parse(tripId));
            return new ResponseMsg {
                status = result > 0 ? true : false,
                data = null,
                message = result > 0 ? "Cancel trip successfully" : "Failed to cancel this trip"
            };
            
        }

        [HttpGet]
        public async Task<ResponseMsg> GetPassengerTrips([FromBody] object passengerIdJson)
        {
            JObject objTemp = JObject.Parse(passengerIdJson.ToString());
            string passengerId = (string)objTemp["passengerId"];
            List<Models.Trip> trips = await Repository.Trip.GetListTripsByPassenger(Guid.Parse(passengerId));
            return new ResponseMsg
            {
                status = trips != null ? true : false,
                data = trips,
                message = trips != null ? "Get trips by passenger successfully" : "Failed to get trips by passenger"
            };
        }

        [HttpGet]
        public async Task<ResponseMsg> GetDriverTrips([FromBody] object driverIdJson)
        {
            JObject objTemp = JObject.Parse(driverIdJson.ToString());
            string driverId = (string)objTemp["driverId"];
            List<Models.Trip> trips = await Repository.Trip.GetListTripsByDriver(Guid.Parse(driverId));
            return new ResponseMsg
            {
                status = trips != null ? true : false,
                data = trips,
                message = trips != null ? "Get trips by driver successfully" : "Failed to get trips by driver"
            };
        }

        [HttpGet]
        public async Task<ResponseMsg> GetCurrentTrip([FromBody]object tripIdJson)
        {
            JObject objTemp = JObject.Parse(tripIdJson.ToString());
            string tripId = (string)objTemp["tripId"];
            Models.Trip result = await Repository.Trip.GetTrip(Guid.Parse(tripId));
            return new ResponseMsg
            {
                status = result != null ? true : false,
                data = result,
                message = result != null ? "Get trip successfully" : "Failed to get trip",
            };
        }



        [HttpGet]
        public async Task<ResponseMsg> GetCurrentTripForPassenger([FromBody]object passengerTrip)
        {
            JObject objTemp = JObject.Parse(passengerTrip.ToString());
            string passengerId = (string)objTemp["passengerId"];
            string requestId = (string)objTemp["requestId"];
            
            Models.Trip trip = await Repository.Trip.GetTripForPassenger(Guid.Parse(passengerId), Guid.Parse(requestId));
            return new ResponseMsg
            {
                status = trip != null ? true : false,
                data = trip,
                message = trip != null ? "Get trip successfully" : "Failed to get trip",
            };
        }

        [HttpPost]
        public async Task<ResponseMsg> PickedPassenger([FromBody] object tripIdJson)
        {
            JObject objTemp = JObject.Parse(tripIdJson.ToString());
            string tripId = (string)objTemp["tripId"];
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


        [HttpGet]
        public async Task<ResponseMsg> GetIncome([FromBody]object getIncome)
        {
            JObject objTemp = JObject.Parse(getIncome.ToString());
            string driverId = (string)objTemp["driverId"];
            string from = (string)objTemp["from"];
            string to = (string)objTemp["to"];

            int income = await Repository.Trip.GetIncome(Guid.Parse(driverId), from, to);
            return new ResponseMsg
            {
                status = true,
                data = income,
                message = "Get income successfully"
            };
        }

        [HttpGet]
        public async Task<ResponseMsg> GetCompletedTrips([FromBody]object getCompletedTrips)
        {
            JObject objTemp = JObject.Parse(getCompletedTrips.ToString());
            string driverId = (string)objTemp["driverId"];
            string from = (string)objTemp["from"];
            string to = (string)objTemp["to"];

            var result = await Repository.Trip.GetCompletedTrips(Guid.Parse(driverId), from, to);
            if (result != null)
            {
                return new ResponseMsg
                {
                    status = true,
                    data = result,
                    message = "Get completed trips successfully"
                };
            }
            return new ResponseMsg
            {
                status = false,
                data = new
                {
                    total = 0,
                    trips = new List<string> { }
                },
                message = "Failed to get completed trips"
            };
        }

        [HttpGet]
        public async Task<ResponseMsg> ClearDb()
        {
            await Repository.Trip.ClearTable();
            await Repository.TripRequest.ClearTable();
            await Repository.TripFeedBack.ClearTable();

            return new ResponseMsg
            {
                status = true,
                data = null,
                message = "Executed clear all database of Trip service"
            };
        }
    }
}
