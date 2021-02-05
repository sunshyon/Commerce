using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Base.JWT
{
    /// <summary>
    /// RSA非对称加密 -- 会生成key文件
    /// </summary>
    public class JwtRSAService : IJwtService
    {
        private readonly JwtOptions _jwtOptions;

        public JwtRSAService(IOptionsMonitor<JwtOptions> optionsMonitor)
        {
            this._jwtOptions = optionsMonitor.CurrentValue;
        }
        public string GetToken(string userName)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name,userName),
            };
            var keyDir = Directory.GetCurrentDirectory();
            if (RSAHelper.TryGetKeyParameters(keyDir, true, out RSAParameters keyParams) == false)
            {
                keyParams = RSAHelper.GenerateAndSaveKey(keyDir);
            }
            var creds = new SigningCredentials(new RsaSecurityKey(keyParams), SecurityAlgorithms.RsaSha256Signature);

            var token = new JwtSecurityToken(
                issuer: this._jwtOptions.Issuer,
                audience: this._jwtOptions.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(this._jwtOptions.Expires),//有效期时间
                //notBefore: DateTime.Now.AddMinutes(1),//1分钟后有效
                signingCredentials: creds);

            var resToken = new JwtSecurityTokenHandler().WriteToken(token);
            return resToken;
        }
    }
}
