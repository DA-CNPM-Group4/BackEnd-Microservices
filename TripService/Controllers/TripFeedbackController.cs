using Helper.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using TripService.DTOs;
using TripService.Models;

namespace TripService.Controllers
{
    [Route("api/Trip/[controller]/[action]")]
    [ApiController]
    public class TripFeedbackController : BaseController
    {
        [HttpPost]
        public async Task<ResponseMsg> RateTrip(RateTripDTO rateTripDTO)
        {
            int result = await Repository.TripFeedBack.RateTrip(Guid.Parse(rateTripDTO.TripId), rateTripDTO.Description, rateTripDTO.Rate);
            return new ResponseMsg
            {
                status = result > 0 ? true : false,
                data = null,
                message = result > 0 ? "Rate trip feedback successfully" : "Failed to rate trip"
            };
        }

            
        [HttpGet]
        public async Task<ResponseMsg> GetTripFeedBack([FromBody]object tripIdJson)
        {
            JObject objTemp = JObject.Parse(tripIdJson.ToString());
            string tripId = (string)objTemp["tripId"];
            TripFeedback tripFeedback = await Repository.TripFeedBack.GetTripFeedback(Guid.Parse(tripId));
            return new ResponseMsg
            {
                status = tripFeedback is not null ? true : false,
                data = tripFeedback,
                message = tripFeedback is not null ? "Get trip successfully" : "Failed to get trip"
            };
        }
    }
}
