using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Base.JWT
{
    public class JwtHS256Service:IJwtService
    {
        private readonly JwtOptions _jwtOptions;

        public JwtHS256Service(IOptionsMonitor<JwtOptions> optionsMonitor)
        {
            this._jwtOptions = optionsMonitor.CurrentValue;
        }
        public string GetToken(string userName)
        {
            userName = userName== null? "": userName;
            var claims = new[]
            {
                new Claim(ClaimTypes.Name,userName),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this._jwtOptions.SecurityKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: this._jwtOptions.Issuer,
                audience: this._jwtOptions.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(this._jwtOptions.Expires),//有效期
                //notBefore: DateTime.Now.AddMinutes(1),//1分钟后有效
                signingCredentials: creds);

            var resToken = new JwtSecurityTokenHandler().WriteToken(token);
            return resToken;

        }
    }
}
