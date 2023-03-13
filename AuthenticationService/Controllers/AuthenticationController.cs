using AuthenticationService.Models;
using AuthenticationService.RabbitMQServices;
using Helper;
using Helper.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Data;
using static Helper.Catalouge;
using System.Numerics;

namespace AuthenticationService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthenticationController : BaseController
    {
        private readonly IMessageProducer _producer;
        public AuthenticationController(IMessageProducer messageProducer)
        {
            _producer = messageProducer;
        }

        [HttpPost]
        public async Task<ResponseMsg> Login(object loginInfo)
        {
            AuthenticationInfo info = await Repository.Authentication.Login(loginInfo);

            if (info is not null)
            {
                _producer.SendMessage("authenInfo", new
                {
                    Status = true,
                    Message = $"GetDataInfo{info.Role}",
                    Data = info.AccountId
                });
                return new ResponseMsg
                {
                    status = true,
                    data = null,
                    message = "Login success"
                };
            }
            else
            {
                return new ResponseMsg
                {
                    status = false,
                    data = null,
                    message = "Login failed, please check your input again"
                };
            }
        }

        [HttpPost]
        public async Task<ResponseMsg> Register(object registerInfo)
        {
            JObject objTemp = JObject.Parse(registerInfo.ToString());
            AuthenticationInfo registerObj = new AuthenticationInfo
            {
                AccountId = new Guid(),
                Name = (string)objTemp["name"],
                Email = (string)objTemp["email"],
                Phone = (string)objTemp["phone"],
                Password = (string)objTemp["password"],
                Role = (string)objTemp["role"]
            };

            if(await Repository.Authentication.Register(registerObj)  == 0)
            {
                return new ResponseMsg
                {
                    status = false,
                    data = null,
                    message = "Register failed, try another email"
                };
            }
            else
            {
                _producer.SendMessage("authenInfo", new
                {
                    Status = true,
                    Message = "AddDataInfo",
                    Data = registerObj,
                });
                return new ResponseMsg
                {
                    status = true,
                    data = null,
                    message = "Register success, please login"
                };
            }
        }
    }
}
