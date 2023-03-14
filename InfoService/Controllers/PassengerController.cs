using Helper.Models;
using InfoService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace InfoService.Controllers
{
    [Route("api/Info/[controller]/[action]")]
    [ApiController]
    public class PassengerController : BaseController
    {
        [HttpGet]
        public async Task<ResponseMsg> GetPassengers()
        {
            return new ResponseMsg
            {
                status = true,
                data = await Repository.Passenger.GetAllPassengers(),
                message = "Get all passenger info success"
            };
        }

        [HttpPost]
        public async Task<ResponseMsg> AddInfo(Passenger passenger)
        {
            int result = await Repository.Passenger.AddPassengerInfo(passenger);
            if (result > 0)
            {
                return new ResponseMsg
                {
                    status = true,
                    data = new { accountId = passenger.AccountId },
                    message = "Add passenger info success"
                };
            }
            else
            {
                return new ResponseMsg
                {
                    status = false,
                    data = null,
                    message = "Add passenger info failed"
                };
            }
        }

        [HttpPost]
        public async Task<ResponseMsg> GetPassengerInfoById(object accountObj)
        {
            JObject objTemp = JObject.Parse(accountObj.ToString());
            string id = (string)objTemp["accountId"];
            Passenger passenger = await Repository.Passenger.GetPassengerById(Guid.Parse(id));
            if (passenger is null)
            {
                return new ResponseMsg
                {
                    status = false,
                    data = null,
                    message = "Get driver info failed, staff does not exist"
                };
            }
            else
            {
                return new ResponseMsg
                {
                    status = false,
                    data = passenger,
                    message = "Get driver info success"
                };
            }
        }

        [HttpPost]
        public async Task<ResponseMsg> UpdateInfo(Passenger passenger)
        {
            if (await Repository.Passenger.CheckPassengerExist(passenger.AccountId))
            {
                int res = await Repository.Passenger.UpdatePassengerInfo(passenger);
                if (res > 0)
                {
                    return new ResponseMsg
                    {
                        status = true,
                        data = null,
                        message = "Update passenger info success"
                    };
                }
                return new ResponseMsg
                {
                    status = false,
                    data = null,
                    message = "Update failed, nothing changed"
                };
            }
            else
            {
                return new ResponseMsg
                {
                    status = false,
                    data = null,
                    message = "Update passenger info failed, passenger does not exist"
                };
            }
        }
    }


}
