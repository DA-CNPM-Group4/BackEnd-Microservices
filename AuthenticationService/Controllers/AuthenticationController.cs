using AuthenticationService.Models;
//using AuthenticationService.RabbitMQServices;
using Helper;
using Helper.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Data;
using static Helper.Catalouge;
using System.Numerics;
using System.Security.Claims;
using JwtTokenManager;
using Microsoft.AspNetCore.Authorization;

namespace AuthenticationService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthenticationController : BaseController
    {
        private readonly TokenHandler _tokenHandler;
        public AuthenticationController(TokenHandler tokenHandler)
        {
            _tokenHandler = tokenHandler;
        }

        //private readonly IMessageProducer _producer;
        //public AuthenticationController(IMessageProducer messageProducer)
        //{
        //    _producer = messageProducer;
        //}

        [HttpGet]
        public async Task<string> GetRandomString()
        {
            return "This is an random string";
        }

        private async Task<Token> SaveUserInfoAndCreateTokens(AuthenticationInfo usr)
        {
            ClaimsIdentity claims = new ClaimsIdentity(new Claim[]
               {
                    new Claim(ClaimTypes.Name, usr.Name),
                    new Claim(ClaimTypes.Email, usr.Email),
                    new Claim(ClaimTypes.NameIdentifier, usr.AccountId.ToString()),
               });
            Token token = new Token
            {
                AccessToken = _tokenHandler.CreateAccessToken(claims),
                RefreshToken = _tokenHandler.CreateRefreshToken()
            };
            int saveToDbResult = await Repository.Authentication.UpdateUserTokens(usr.AccountId, token.RefreshToken, DateTime.UtcNow.AddDays(7));
            return token;
        }

        [HttpPost]
        public async Task<ResponseMsg> Login(object loginInfo)
        {
            AuthenticationInfo info = await Repository.Authentication.Login(loginInfo);

            if (info is not null)
            {
                //_producer.SendMessage("info", new
                //{
                //    status = true,
                //    message = $"getdatainfo{info.Role}",
                //    data = info.AccountId
                //});
                Token token = await SaveUserInfoAndCreateTokens(info);
                return new ResponseMsg
                {
                    status = true,
                    data = new {
                        accountId = info.AccountId,
                        accessToken = token.AccessToken,
                        refreshToken = token.RefreshToken
                    },
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
            int registerResult = await Repository.Authentication.Register(registerInfo);
            if (registerResult <= 0)
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
                //_producer.SendMessage("info", new
                //{
                //    Status = true,
                //    Message = "AddDataInfo",
                //    Data = registerObj,
                //});
                return new ResponseMsg
                {
                    status = true,
                    data = null,
                    message = "Register success, please login"
                };
            }
        }

        [HttpPost, Authorize]
        public async Task<ResponseMsg> ChangePassword(object changePasswordObj)
        {
            JObject objTemp = JObject.Parse(changePasswordObj.ToString());
            string currentPassword = (string)objTemp["currentPassword"];
            string newPassword = (string)objTemp["newPassword"];

            Guid UserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            bool validatePassword = await Repository.Authentication.ValidatePassword(UserId, currentPassword);
            if (validatePassword == true)
            {
                int result = await Repository.Authentication.ChangePassword(UserId, newPassword);
                return new ResponseMsg
                {
                    status = result > 0 ? true : false,
                    data = null,
                    message = result > 0 ? "Change user password success" : "Change user password failed"
                };
            }
            return new ResponseMsg
            {
                status = false,
                data = null,
                message = "Change user password failed"
            };
        }

        [HttpGet, Authorize]
        public async Task<ResponseMsg> Logout()
        {
            Guid UserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            int logoutResult = await Repository.Authentication.ClearUsrToken(UserId);
            return new ResponseMsg
            {
                status = true,
                data = null,
                message = logoutResult > 0 ? "Logout success" : "Logout failed"
            };
        }
    }
}
