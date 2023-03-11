using AuthenticationService.Models;
using Helper.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace AuthenticationService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly AuthDbContext _context;
        public AuthenticationController(AuthDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ResponseMsg> Login(object loginInfo)
        {
            JObject objTemp = JObject.Parse(loginInfo.ToString());
            string email = (string)objTemp["email"];
            string phone = (string)objTemp["phone"];
            string password = (string)objTemp["password"];
            string role = (string)objTemp["role"];

            if (await _context.AuthenticationInfo.AnyAsync(p => p.Email == email && p.Phone == phone && p.Password == password))
            {
                return new ResponseMsg
                {
                    status = true,
                    data = await _context.AuthenticationInfo.Where<AuthenticationInfo>(p => p.Email == email).SingleOrDefaultAsync(),
                    message = "Login success"
                };
            }
            else
            {
                return new ResponseMsg
                {
                    status = false,
                    data = null,
                    message = "Login failed"
                };
            }
        }

        [HttpPost]
        public async Task<ResponseMsg> Register(object registerInfo)
        {
            JObject objTemp = JObject.Parse(registerInfo.ToString());
            string email = (string)objTemp["email"];
            string phone = (string)objTemp["phone"];
            string password = (string)objTemp["password"];
            string role = (string)objTemp["role"];

            if (_context.AuthenticationInfo.Any(p => p.Email == email))
            {
                return new ResponseMsg
                {
                    status = false,
                    data = null,
                    message = "Register failed, email already exist"
                };
            }
            else
            {
                await _context.AddAsync(new AuthenticationInfo
                {
                    Name = "",
                    Password = password,
                    Email = email,
                    Phone = phone,
                    IsThirdPartyAccount = false,
                    Role = role,
                    ValidateString = "",
                    IsValidated = true,
                    RefreshToken = "",
                });

                int registerResult = await _context.SaveChangesAsync();
                if(registerResult > 0)
                {
                    return new ResponseMsg
                    {
                        status = true,
                        data = null,
                        message = "Register success, please login"
                    };
                }
                else
                {
                    return new ResponseMsg
                    {
                        status = false,
                        data = null,
                        message = "Register failed, please try again"
                    };
                }
            }
        }
    }
}
