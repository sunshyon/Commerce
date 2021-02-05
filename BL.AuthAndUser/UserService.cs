using Base.ThirdTool;
using BL.Contracts;
using Domain.DbModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.AuthAndUser
{
    [Route("/api/user/[action]")]
    public class UserService: BaseService
    {
        private readonly OrangeContext _db;
        private readonly RabbitMQClient _rabbit;

        public UserService(OrangeContext orangeContext,
            RabbitMQClient rabbit)
        {
            _db = orangeContext;
            _rabbit = rabbit;
        }
        [HttpGet]
        public object GetAll()
        {
            
            var users = _db.TbUser.ToList();
            ajaxRes.Value = users;
            return ajaxRes;
        }
    }
}
