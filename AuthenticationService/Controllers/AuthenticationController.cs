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
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System;
using Azure.Core;
using System.Security.Cryptography.Xml;

namespace AuthenticationService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthenticationController : BaseController
    {
        private HttpClient _httpClient = null;
        private HttpClient httpClient => _httpClient ?? (new HttpClient());
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

        [HttpPost]
        public async Task<ResponseMsg> RetriveTokens(Token token)
        { 
            string userId = _tokenHandler.GetUserIdFromExpiredToken(token.AccessToken);
            if(userId is not null)
            {
                AuthenticationInfo userInfo = await Repository.Authentication.ValidateRefreshToken(token.AccessToken, Guid.Parse(userId));
                if (userInfo is not null)
                {
                    Token newToken = await SaveUserInfoAndCreateTokens(userInfo);

                    return new ResponseMsg
                    {
                        status = true,
                        data = new
                        {
                            accountId = userInfo.AccountId,
                            accessToken = token.AccessToken,
                            refreshToken = token.RefreshToken
                        },
                        message = "Refresh access token success"
                    };
                }
                else
                {
                    return new ResponseMsg
                    {
                        status = false,
                        data = null,
                        message = "Invalidate refresh token or refresh token has been expired, please login again"
                    };
                }
            }
            else{
                return new ResponseMsg
                {
                    status = false,
                    data = null,
                    message = "Invalid access token, try again or re-login"
                };
            }
        }

        public async Task<bool> VerifyGoogleToken(string url)
        {
            try
            {
                var msg = new HttpRequestMessage(HttpMethod.Get, url);
                //msg.Headers.Add("User-Agent", "C# Program");
                var res = httpClient.Send(msg);

                string content = await res.Content.ReadAsStringAsync();
                JObject objTemp2 = JObject.Parse(content);
                string error = (string)objTemp2["error_description"];
                if(error != null)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }

        public async Task<string> GetGoogleAccountInfo(string url)
        {
            try
            {
                var msg = new HttpRequestMessage(HttpMethod.Get, url);
                //msg.Headers.Add("User-Agent", "C# Program");
                var res = httpClient.Send(msg);

                string content = await res.Content.ReadAsStringAsync();
                return content;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }

        public async Task<ResponseMsg> LoginWithGoogle(object loginToken)
        {
            JObject objTemp = JObject.Parse(loginToken.ToString());
            string googleToken = (string)objTemp["loginToken"];
            string role = (string)objTemp["role"];
            string urlVerify = $"https://www.googleapis.com/oauth2/v3/tokeninfo?access_token={googleToken}";
            bool tokenIsValidated = await VerifyGoogleToken(urlVerify);
            if(tokenIsValidated == true)
            {
                string urlGetInfo = $"https://www.googleapis.com/oauth2/v3/userinfo?access_token={googleToken}";
                string responecontent = await GetGoogleAccountInfo(urlGetInfo);
                JObject objTemp2 = JObject.Parse(responecontent);
                string name = (string)objTemp2["name"];
                string email = (string)objTemp2["email"];
                string picture = (string)objTemp2["picture"];

                if (await Repository.Authentication.CheckEmailExisted(email) == true)
                {
                    AuthenticationInfo usr = await Repository.Authentication.LoginWithEmail(email);
                    //if (usr.Name != name)
                    //{
                    //    await Repository.Authentication.UpdateUserInfo(usr.AccountId, name);
                    //}
                    Token token = await SaveUserInfoAndCreateTokens(usr);
                    return new ResponseMsg
                    {
                        status = true,
                        data = new
                        {
                            accountId = usr.AccountId,
                            accessToken = token.AccessToken,
                            refreshToken = token.RefreshToken
                        },
                        message = "Login success"
                    };
                }
                else
                {
                    int registerResult = await Repository.Authentication.RegisterWithGoogleInfo(email, name, role);
                    if (registerResult > 0)
                    {
                        AuthenticationInfo usr = await Repository.Authentication.LoginWithEmail(email);
                        Token token = await SaveUserInfoAndCreateTokens(usr);
                        return new ResponseMsg
                        {
                            status = true,
                            data = new
                            {
                                accountId = usr.AccountId,
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
                            message = "Login failed, failed to save google user info into database"
                        };
                    }
                }
            }

            return new ResponseMsg
            {
                status = false,
                data = null,
                message = "Login with google account failed, google token is invalid"
            };
        }

        //public async Task<ResponseMsg> GoogleLogin(GoogleLoginModel loginModel)
        //{
        //    var handler = new JwtSecurityTokenHandler();
        //    var jwtSecurityToken = handler.ReadJwtToken(loginModel.GoogleCredential);
        //    string email = jwtSecurityToken.Claims.First(claim => claim.Type == "email").Value;
        //    string name = jwtSecurityToken.Claims.First(claim => claim.Type == "name").Value;
        //    string avatar = jwtSecurityToken.Claims.First(claim => claim.Type == "picture").Value;
        //    if (await Repository.Authentication.CheckEmailExisted(email) == true)
        //    {
        //        AuthenticationInfo usr = await Repository.Authentication.LoginWithEmail(email);
        //        if (usr.Name != name)
        //        {
        //            await Repository.Authentication.UpdateUserInfo(usr.AccountId, name);
        //        }
        //        Token token = await SaveUserInfoAndCreateTokens(usr);
        //        return new ResponseMsg
        //        {
        //            status = true,
        //            data = token,
        //            message = "Login success"
        //        };
        //    }
        //    else
        //    {
        //        int registerResult = await Repository.Authentication.RegisterWithGoogleInfo(email, name);
        //        if (registerResult > 0)
        //        {
        //            AuthenticationInfo usr = await Repository.Authentication.LoginWithEmail(email);
        //            Token token = await SaveUserInfoAndCreateTokens(usr);
        //            return new ResponseMsg
        //            {
        //                status = true,
        //                data = token,
        //                message = "Login success"
        //            };
        //        }
        //        else
        //        {
        //            return new ResponseMsg
        //            {
        //                status = false,
        //                data = null,
        //                message = "Login failed"
        //            };
        //        }
        //    }
        //}
    }
}
