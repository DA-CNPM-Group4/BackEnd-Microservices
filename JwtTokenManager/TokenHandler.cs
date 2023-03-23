using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JwtTokenManager
{
    public class TokenHandler
    {
        public const string JWT_SECURITY_KEY = "12312dasdasd2312ffdasfsdfsdfadfIKDASKDndasdqweasdnA";
        public string CreateAccessToken(ClaimsIdentity claims)
        {
            byte[] key = Encoding.ASCII.GetBytes(JWT_SECURITY_KEY);
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddDays(3),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };
            var token = handler.CreateToken(descriptor);
            return handler.WriteToken(token);
        }

        public string CreateRefreshToken()
        {
            var RefreshToken = Helper.DoStuff.RandomString(32);
            return RefreshToken;
        }

        public Guid GetUserIdFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWT_SECURITY_KEY)),
                ValidateLifetime = false //don't care about the token's expiration date
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
            return Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier));
        }
    }
}
