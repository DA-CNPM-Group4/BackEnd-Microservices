using Helper.Models;
using InfoService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace InfoService.Controllers
{
    [Route("api/Info/[controller]/[action]")]
    [ApiController]
    public class StaffController : BaseController
    {
        [HttpGet]
        public async Task<ResponseMsg> GetStaffs()
        {
            return new ResponseMsg
            {
                status = true,
                data = await Repository.Staff.GetAllStaffs(),
                message = "Get all staff info success"
            };
        }

        [HttpPost]
        public async Task<ResponseMsg> AddInfo(Staff staff)
        {
            int result = await Repository.Staff.AddStaffInfo(staff);
            if (result > 0)
            {
                return new ResponseMsg
                {
                    status = true,
                    data = new { accountId = staff.AccountId },
                    message = "Add staff info success"
                };
            }
            else
            {
                return new ResponseMsg
                {
                    status = false,
                    data = null,
                    message = "Add staff info failed"
                };
            }
        }

        [HttpPost]
        public async Task<ResponseMsg> GetStaffInfoById(object accountObj)
        {
            JObject objTemp = JObject.Parse(accountObj.ToString());
            string id = (string)objTemp["accountId"];
            Staff staff = await Repository.Staff.GetStaffById(Guid.Parse(id));
            if(staff is null)
            {
                return new ResponseMsg
                {
                    status = false,
                    data = null,
                    message = "Get staff info failed, staff does not exist"
                };
            }
            else
            {
                return new ResponseMsg
                {
                    status = false,
                    data = staff,
                    message = "Get staff info success"
                };
            }
        }

        [HttpPost]
        public async Task<ResponseMsg> UpdateInfo(Staff staff)
        {
            if (await Repository.Staff.CheckStaffExist(staff.AccountId))
            {
                int res = await Repository.Staff.UpdateStaffInfo(staff);
                if (res > 0)
                {
                    return new ResponseMsg
                    {
                        status = true,
                        data = null,
                        message = "Update staff info success"
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
                    message = "Update staff info failed, staff does not exist"
                };
            }
        }
    }
}
