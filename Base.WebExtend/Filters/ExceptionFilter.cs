using Base.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.WebExtend.Filters
{
    public class ExceptionFilter: IExceptionFilter
    {
        private ILogger<ExceptionFilter> _logger = null;
        public ExceptionFilter(ILogger<ExceptionFilter> logger)
        {
            this._logger = logger;
        }
        public void OnException(ExceptionContext context)
        {
            if (context.ExceptionHandled == false)
            {
                context.Result = new JsonResult(new AjaxResult()
                {
                    Message = "操作失败",
                    OtherValue = context.Exception.Message,
                    Result = false
                });
                this._logger.LogError(context.Exception.Message);
            }
            context.ExceptionHandled = true;
        }
    }
}
