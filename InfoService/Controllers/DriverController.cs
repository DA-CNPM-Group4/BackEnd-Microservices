using InfoService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InfoService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriverController : BaseController
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Driver>>> GetDrivers()
        {
            return await provider.Driver.GetDrivers();
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create(Driver driver)
        {
            return await provider.Driver.CreateDriver(driver);
        }
    }
}
