using Base.Common.Models;
using Microsoft.AspNetCore.Cors;
using System;

namespace BL.Contracts
{
    public class BaseService:IBaseService
    {
        public AjaxResult ajaxRes;
        public BaseService()
        {
            ajaxRes = new AjaxResult();
        }
    }
}
