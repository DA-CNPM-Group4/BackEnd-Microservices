using Helper.Models;
using InfoService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace InfoService.Controllers
{
    [Route("api/Info/[controller]/[action]")]
    [ApiController]
    public class DriverController : BaseController
    {
        [HttpGet]
        public async Task<ResponseMsg> GetDrivers()
        {
            return new ResponseMsg {
                status = true,
                data = await Repository.Driver.GetAllDrivers(),
                message = "Get all driver info success"
            };
        }

        [HttpPost]
        public async Task<ResponseMsg> AddInfo(Driver driver)
        {
            return new ResponseMsg
            {
                status = true,
                data = await Repository.Driver.AddDriverInfo(driver),
                message = "Create driver info success"
            };
        }

        [HttpPost]
        public async Task<ResponseMsg> GetDriverInfoById(object accountObj)
        {
            JObject objTemp = JObject.Parse(accountObj.ToString());
            string id = (string)objTemp["accountId"];
            Driver driver = await Repository.Driver.GetDriverById(Guid.Parse(id));
            if (driver is null)
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
                    data = driver,
                    message = "Get driver info success"
                };
            }
        }

        [HttpPost]
        public async Task<ResponseMsg> UpdateInfo(Driver driver)
        {
            if(await Repository.Driver.CheckDriverExist(driver.AccountId))
            {
                int res = await Repository.Driver.UpdateDriverInfo(driver);
                if(res > 0)
                {
                    return new ResponseMsg
                    {
                        status = true,
                        data = null,
                        message = "Update driver info success"
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
                    message = "Get update driver info failed, driver does not exist"
                };
            }
        }
    }
}
