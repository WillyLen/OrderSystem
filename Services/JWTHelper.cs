using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace OrderSystem.Services
{
    public class JwtHelper
    {
        private static string secretKey = ConfigurationManager.AppSettings["JwtSecretKey"]; // 從Web.config取得密鑰

        /// <summary>
        /// 產生JWT Token，時限為30分鐘(過期會使用Refresh Token重新送一組)
        /// </summary>
        public static string GenerateToken(string account, string role, int expireMinutes = 30)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, account),
                    new Claim(ClaimTypes.Role, role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// 驗證Token
        /// </summary>
        public static ClaimsPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var parameters = new TokenValidationParameters 
            {
                ValidateIssuerSigningKey = true,                  // 是否驗證簽章密鑰
                IssuerSigningKey = new SymmetricSecurityKey(key), // 使用的簽章密鑰
                ValidateIssuer = false,                           // 是否驗證發行者
                ValidateAudience = false,                         // 是否驗證接收者
                ClockSkew = TimeSpan.Zero                         // 時間誤差容許範圍(此處是0)
            };
            return tokenHandler.ValidateToken(token, parameters, out _);
        }
    }
}
