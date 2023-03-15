using Helper.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TripService.Models;
using TripService.Repositories;

namespace TripService.Controllers
{
    [Route("api/Trip/[controller]/[action]")]
    [ApiController]
    public class TripRequestController : BaseController
    {
        [HttpPost]
        public async Task<ResponseMsg> SendRequest(TripRequest request)
        {
            request.RequestId = Guid.NewGuid();
            request.CreatedTime = DateTime.Now;
            int result = await Repository.TripRequest.CreateRequest(request);
            return new ResponseMsg
            {
                status = result > 0 ? true : false,
                data =  result > 0 ? request.RequestId : null,
                message = result > 0 ? "Send request successfully":"Failed to send request",
            };
        }

        [HttpGet]
        public async Task<ResponseMsg> CalculatePrice(double distance)
        {
            return new ResponseMsg
            {
                status = true,
                data = Repository.TripRequest.CalcPrice(distance),
                message = "Price base on distance and vehicle type",
            };
        }

        [HttpGet]
        public async Task<ResponseMsg> CancelRequestByPassenger(string requestId)
        {
            return new ResponseMsg
            {
                status = true,
                data = null,
                message = null,
            };
        }

        [HttpGet]
        public async Task<ResponseMsg> CancelRequestByDriver(string requestId)
        {
            return new ResponseMsg
            {
                status = true,
                data = null,
                message = null,
            };
        }
    }
}
