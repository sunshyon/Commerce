using Base.Common.Models;
using Base.JWT;
using Base.WebExtend.Filters;
using BL.Contracts;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.AuthAndUser
{

    [Route("/api/auth/[action]")]
    public class AuthService : BaseService
    {
        private readonly IJwtService _jwtService;

        public AuthService(IJwtService jwtService)
        {
            _jwtService = jwtService;
        }
        [HttpPost]
        public object Accredit(string username,string password)
        {
            if (username == "admin" && password == "123")
            {
                var token= _jwtService.GetToken(username);
                return token;
            }
            return "";
        }
        [HttpGet]
        [TypeFilter(typeof(RepeatSubmitFilter))]//防止重复提交
        public string GetToken(string username)
        {
            var token = _jwtService.GetToken(username);
            return token;
        }
    }

}
