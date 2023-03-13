using Helper.Models;
using InfoService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InfoService.Controllers
{
    [Route("api/[controller]/[action]")]
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
        public async Task<ResponseMsg> Create(Driver driver)
        {
            return new ResponseMsg
            {
                status = true,
                data = await Repository.Driver.AddDriverInfo(driver),
                message = "Create driver info success"
            };
        }
    }
}
