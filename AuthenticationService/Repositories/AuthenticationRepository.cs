using AuthenticationService.Models;
using Helper.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace AuthenticationService.Repositories
{
    public class AuthenticationRepository : BaseRepository
    {
        public AuthenticationRepository(AuthenticationDbContext context) : base(context)
        {
        }

        public async Task<int> Register(object registerInfo)
        {
            JObject objTemp = JObject.Parse(registerInfo.ToString());
            AuthenticationInfo registerData = new AuthenticationInfo
            {
                AccountId = new Guid(),
                Name = (string)objTemp["name"],
                Email = (string)objTemp["email"],
                Phone = (string)objTemp["phone"],
                Role = (string)objTemp["role"]
            };
            string registerPassword = (string)objTemp["password"];
            if(await CheckEmailAndPhoneNumExisted(registerData.Email, registerData.Phone) == true)
            {
                return 0;
            }

            registerData.Password = Helper.DoStuff.HashString(registerData.Email + "^@#%!@(!&^$" + registerPassword);
            await context.AddAsync(registerData);
            return await context.SaveChangesAsync();
        }

        public async Task<AuthenticationInfo> Login(object loginInfo)
        {
            JObject objTemp = JObject.Parse(loginInfo.ToString());
            string loginEmail = (string)objTemp["email"];
            string loginPhone = (string)objTemp["phone"];
            string loginPassword = (string)objTemp["password"];

            var usr = from user in context.AuthenticationInfo
                      where user.Email == loginEmail && user.Phone == loginPhone && (user.Password.SequenceEqual(Helper.DoStuff.HashString(loginEmail + "^@#%!@(!&^$" + loginPassword)))
                      select user;
            return await usr.SingleOrDefaultAsync();
        }

        public async Task<AuthenticationInfo> GetUserById(Guid UserId)
        {
            AuthenticationInfo usr = await context.AuthenticationInfo.FindAsync(UserId);
            return usr;
        }


        public async Task<int> UpdateUserTokens(Guid UserId, string RefreshToken, DateTime expiredTime)
        {
            AuthenticationInfo usr = await GetUserById(UserId);
            if (usr != null)
            {
                usr.RefreshToken = RefreshToken;
                usr.RefreshTokenExpiredDate = expiredTime;
                try
                {
                    return await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return 0;
        }

        public async Task<bool> CheckEmailAndPhoneNumExisted(string email, string phone)
        {
            bool existed1 = await context.AuthenticationInfo.AnyAsync(p => p.Email == email);
            bool existed2 = await context.AuthenticationInfo.AnyAsync(p => p.Phone == phone);
            return existed1 || existed2;
        }

        public async Task<bool> ValidatePassword(Guid userId, string password)
        {
            AuthenticationInfo usr = await context.AuthenticationInfo.FindAsync(userId);
            if (usr.Password.SequenceEqual(Helper.DoStuff.HashString(usr.Email + "^@#%!@(!&^$" + password)))
            {
                return true;
            }
            return false;
        }
        public async Task<int> ChangePassword(Guid userId, string newPassword)
        {
            AuthenticationInfo usr = await context.AuthenticationInfo.FindAsync(userId);
            usr.Password = Helper.DoStuff.HashString(usr.Email + "^@#%!@(!&^$" + newPassword);
            return await context.SaveChangesAsync();
        }

        public async Task<int> ClearUsrToken(Guid UserId)
        {
            AuthenticationInfo usr = context.AuthenticationInfo.Where<AuthenticationInfo>(p => p.AccountId == UserId).SingleOrDefault();
            if (usr != null)
            {
                usr.RefreshToken = String.Empty;
                usr.RefreshTokenExpiredDate = DateTime.UtcNow;
            }
            return await context.SaveChangesAsync();
        }
        public async Task<AuthenticationInfo> ValidateRefreshToken(string RefreshToken, Guid UserId)
        {
            AuthenticationInfo user = await context.AuthenticationInfo.Where(p => p.AccountId == UserId && p.RefreshToken == RefreshToken && p.RefreshTokenExpiredDate < DateTime.UtcNow).SingleOrDefaultAsync();
            return user;
        }

        public async Task<int> RegisterWithGoogleInfo(string email, string name, string role)
        {
            AuthenticationInfo usr = new AuthenticationInfo
            {
                Email = email,
                Role = role,
                Name = name,
                Password = Helper.DoStuff.HashString(email + "^@#%!@(!&^$" + "gggggggg"),
                IsValidated = true
            };
            await context.AuthenticationInfo.AddAsync(usr);
            return await context.SaveChangesAsync();
        }

        public async Task<bool> CheckEmailExisted(string email)
        {
            bool existed = await context.AuthenticationInfo.AnyAsync(p => p.Email == email);
            return existed;
        }

        public async Task<int> UpdateUserInfo(Guid userId, string name)
        {
            AuthenticationInfo res = await GetUserById(userId);
            res.Name = name;
            return await context.SaveChangesAsync();
        }

        public async Task<AuthenticationInfo> LoginWithEmail(string email)
        {
            var usr = from user in context.AuthenticationInfo
                      where user.Email == email
                      select user;
            return await usr.SingleOrDefaultAsync();
        }
    }
}
