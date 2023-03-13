using Helper.Models;
using InfoService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace InfoService.Controllers
{
    [Route("api/Info/[controller]/[action]")]
    [ApiController]
    public class VehicleController : BaseController
    {
        [HttpPost]
        public async Task<ResponseMsg> RegisterVehicle(Vehicle vehicle)
        {
            if(await Repository.Vehicle.CheckDriverHasVehicleAlready(vehicle.DriverId) == true)
            {
                return new ResponseMsg
                {
                    status = false,
                    data = null,
                    message = "Register vehicle failed, driver already registered a vehicle"
                };
            }
            else
            {
                int result = await Repository.Vehicle.RegisterVehicle(vehicle);
                if(result > 0)
                {
                    await Repository.Driver.MarkAlreadyRegisVehicle(vehicle.DriverId);
                    return new ResponseMsg
                    {
                        status = true,
                        data = null,
                        message = "Register vehicle success"
                    };
                }
                return new ResponseMsg
                {
                    status = false,
                    data = null,
                    message = "Register vehicle failed"
                };
            }
        }

        [HttpPost]
        public async Task<ResponseMsg> GetDriverVehicle(object accountId)
        {
            JObject objTemp = JObject.Parse(accountId.ToString());
            string id = (string)objTemp["accountId"];
            Vehicle vehicle = await Repository.Vehicle.GetDriverVehicle(Guid.Parse(id));
            if(vehicle == null)
            {
                return new ResponseMsg
                {
                    status = true,
                    data = null,
                    message = "Driver did not registered any vehicle"
                };
            }
            else
            {
                return new ResponseMsg
                {
                    status = true,
                    data = vehicle,
                    message = "Get driver vehicle success"
                };
            }
        }
    }
}
