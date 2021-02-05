using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Base.WebExtend.Filters
{
    public class LogActionFilter: ActionFilterAttribute
    {
        private ILogger<LogActionFilter> _logger = null;
        public LogActionFilter(ILogger<LogActionFilter> logger)
        {
            this._logger = logger;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string url = context.HttpContext.Request.Path.Value;
            string argument = JsonConvert.SerializeObject(context.ActionArguments);
            this._logger.LogInformation($"{url}----->argument={argument}");
        }
    }
}
