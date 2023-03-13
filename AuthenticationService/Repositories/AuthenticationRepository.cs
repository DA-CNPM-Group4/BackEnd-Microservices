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

        public async Task<int> Register(AuthenticationInfo registerObj)
        {
            if(await context.AuthenticationInfo.AnyAsync(p => p.Email == registerObj.Email))
            {
                return 0;
            }
            else
            {
                await context.AddAsync(registerObj);
                return await context.SaveChangesAsync();
            } 
        }

        public async Task<AuthenticationInfo> Login(object loginInfo)
        {
            JObject objTemp = JObject.Parse(loginInfo.ToString());
            AuthenticationInfo loginObj = new AuthenticationInfo
            {
                Email = (string)objTemp["email"],
                Phone = (string)objTemp["phone"],
                Password = (string)objTemp["password"],
                Role = (string)objTemp["role"]
            };

            if (await context.AuthenticationInfo.AnyAsync(p => p.Email == loginObj.Email && p.Phone == loginObj.Phone && p.Password == loginObj.Password))
            {
                return await context.AuthenticationInfo.Where(p => p.Email == loginObj.Email).FirstOrDefaultAsync();
            }
            else
            {
                return null;
            }
        }
    }
}
